using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(3)]
    public class ReactToEntitySystemHandler : IConventionalSystemHandler
    {
        public readonly IDictionary<ISystem, IDictionary<Guid, IDisposable>> _entitySubscriptions;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IEntityCollectionManager EntityCollectionManager;
        
        public ReactToEntitySystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _entitySubscriptions = new Dictionary<ISystem, IDictionary<Guid, IDisposable>>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToEntitySystem; }

        public void SetupSystem(ISystem system)
        {
            var accessor = EntityCollectionManager.CreateObservableGroup(system.TargetGroup);            
            var entitySubscriptions = new Dictionary<Guid, IDisposable>();
            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);

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
            
            _entitySubscriptions.Add(system, entitySubscriptions);
        }

        public void DestroySystem(ISystem system)
        {
            _systemSubscriptions.RemoveAndDispose(system);
            
            var entitySubscriptions = _entitySubscriptions[system];
            entitySubscriptions.Values.DisposeAll();
            entitySubscriptions.Clear();
            _entitySubscriptions.Remove(system);
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
            _systemSubscriptions.DisposeAll();
            foreach (var entitySubscriptions in _entitySubscriptions.Values)
            {
                entitySubscriptions.Values.DisposeAll();
                entitySubscriptions.Clear();
            }
            _entitySubscriptions.Clear();
        }
    }
}