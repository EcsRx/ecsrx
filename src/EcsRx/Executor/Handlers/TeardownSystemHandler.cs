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
    public class TeardownSystemHandler : IConventionalSystemHandler
    {
        public readonly IPoolManager _poolManager;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public TeardownSystemHandler(IPoolManager poolManager)
        {
            _poolManager = poolManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ITeardownSystem; }

        public void SetupSystem(ISystem system)
        {
            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ITeardownSystem) system;
            var accessor = _poolManager.CreateObservableGroup(system.TargetGroup);
            
            accessor.OnEntityRemoved
                .Subscribe(castSystem.Teardown)
                .AddTo(entityChangeSubscriptions);        
        }

        public void DestroySystem(ISystem system)
        {
            _systemSubscriptions.RemoveAndDispose(system);
        }       

        public void Dispose()
        {
            _systemSubscriptions.DisposeAll();
        }
    }
}