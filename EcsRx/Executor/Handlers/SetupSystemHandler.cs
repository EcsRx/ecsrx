using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Timers;
using EcsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(1)]
    public class SetupSystemHandler : IConventionalSystemHandler
    {
        public IPoolManager PoolManager { get; }

        private readonly IDictionary<ISystem, IDisposable> _subscriptions;
        
        public SetupSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
            _subscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ISetupSystem; }

        public void SetupSystem(ISystem system)
        {
            var entitySubscriptions = new Dictionary<Guid, IDisposable>();
            var entityChangeSubscriptions = new CompositeDisposable();
            _subscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ISetupSystem) system;
            var accessor = PoolManager.CreateObservableGroup(system.TargetGroup);

            accessor.OnEntityAdded
                .Subscribe(x => ProcessEntity(castSystem, x))
                .AddTo(entityChangeSubscriptions);
            
            accessor.OnEntityRemoved
                .Subscribe(x =>
                {
                    entitySubscriptions[x.Id].Dispose();
                    entitySubscriptions.Remove(x.Id);
                })
                .AddTo(entityChangeSubscriptions);
            
            accessor.Entities.ForEachRun(x => ProcessEntity(castSystem, x));            
        }

        public void DestroySystem(ISystem system)
        {
            _subscriptions.RemoveAndDispose(system);
        }

        public void ProcessEntity(ISetupSystem system, IEntity entity)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            
            if (!hasEntityPredicate)
            { system.Setup(entity); }

            var groupPredicate = system.TargetGroup as IHasPredicate;
            
            entity.WaitForPredicateMet(groupPredicate.CanProcessEntity)
                .FirstAsync()
                .Subscribe(system.Setup);
        }

        public void Dispose()
        {
            _subscriptions.DisposeAll();
        }
    }
}