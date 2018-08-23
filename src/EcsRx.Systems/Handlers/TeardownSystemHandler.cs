using System;
using System.Collections.Generic;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.MicroRx;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;

namespace EcsRx.Systems.Handlers
{
    [Priority(1)]
    public class TeardownSystemHandler : IConventionalSystemHandler
    {
        public readonly IEntityCollectionManager EntityCollectionManager;
        public readonly IDictionary<ISystem, IDisposable> SystemSubscriptions;
        
        public TeardownSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
            SystemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is ITeardownSystem; }

        public void SetupSystem(ISystem system)
        {
            var entityChangeSubscriptions = new CompositeDisposable();
            SystemSubscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ITeardownSystem) system;
            var observableGroup = EntityCollectionManager.GetObservableGroup(system.Group);
            
            observableGroup.OnEntityRemoving
                .Subscribe(castSystem.Teardown)
                .AddTo(entityChangeSubscriptions);        
        }

        public void DestroySystem(ISystem system)
        {
            SystemSubscriptions.RemoveAndDispose(system);
        }       

        public void Dispose()
        {
            SystemSubscriptions.DisposeAll();
        }
    }
}