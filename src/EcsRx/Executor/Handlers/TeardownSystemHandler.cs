using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Timers;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(1)]
    public class TeardownSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityCollectionManager EntityCollectionManager;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public TeardownSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ITeardownSystem; }

        public void SetupSystem(ISystem system)
        {
            var entityChangeSubscriptions = new CompositeDisposable();
            _systemSubscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ITeardownSystem) system;
            var accessor = EntityCollectionManager.GetObservableGroup(system.TargetGroup);
            
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