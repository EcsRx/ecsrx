using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EcsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(3)]
    public class ReactToEntitySystemHandler : IConventionalSystemHandler
    {
        private readonly IDictionary<ISystem, IDictionary<Guid, IDisposable>> _systemSubscriptions;
        private readonly IDictionary<ISystem, IDisposable> _subscriptions;
        
        public IPoolManager PoolManager { get; }
        
        public ReactToEntitySystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
            _subscriptions = new Dictionary<ISystem, IDisposable>();
            _systemSubscriptions = new Dictionary<ISystem, IDictionary<Guid, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToEntitySystem; }

        public void SetupSystem(ISystem system)
        {
            var accessor = PoolManager.CreateObservableGroup(system.TargetGroup);            
            var entitySubscriptions = new Dictionary<Guid, IDisposable>();
            var entityChangeSubscriptions = new CompositeDisposable();
            _subscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (IReactToEntitySystem) system;
            
            accessor.OnEntityAdded
                .Subscribe(x =>
                {
                    var entitySubscription = ProcessEntity(castSystem, x);
                    entitySubscriptions.Add(x.Id, entitySubscription);
                })
                .AddTo(entityChangeSubscriptions);
            
            accessor.OnEntityRemoved
                .Subscribe(x =>
                {
                    entitySubscriptions.RemoveAndDispose(x.Id);
                })
                .AddTo(entityChangeSubscriptions);

            foreach (var entity in accessor.Entities)
            {
                var entitySubscription = ProcessEntity(castSystem, entity);
                entitySubscriptions.Add(entity.Id, entitySubscription);
            }
            
            _systemSubscriptions.Add(system, entitySubscriptions);
        }

        public void DestroySystem(ISystem system)
        {
            _subscriptions.RemoveAndDispose(system);
            
            var entitySubscriptions = _systemSubscriptions[system];
            entitySubscriptions.Values.DisposeAll();
            entitySubscriptions.Clear();
            _systemSubscriptions.Remove(system);
        }
        
        public IDisposable ProcessEntity(IReactToEntitySystem system, IEntity entity)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var reactObservable = system.ReactToEntity(entity);
            
            if (!hasEntityPredicate)
            { return reactObservable.Subscribe(system.Execute); }

            var groupPredicate = system.TargetGroup as IHasPredicate;
            return reactObservable
                .Where(groupPredicate.CanProcessEntity)
                .Subscribe(system.Execute);
        }

        public void Dispose()
        {
            _subscriptions.DisposeAll();
            foreach (var entitySubscriptions in _systemSubscriptions.Values)
            {
                entitySubscriptions.Values.DisposeAll();
                entitySubscriptions.Clear();
            }
            _systemSubscriptions.Clear();
        }
    }
}