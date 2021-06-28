using System;
using System.Collections.Generic;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using EcsRx.Collections;
using EcsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Handlers
{
    [Priority(10)]
    public class TeardownSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager ObservableGroupManager;
        public readonly IDictionary<ISystem, IDisposable> SystemSubscriptions;
        
        public TeardownSystemHandler(IObservableGroupManager observableGroupManager)
        {
            ObservableGroupManager = observableGroupManager;
            SystemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem groupSystem)
        { return groupSystem is ITeardownSystem; }

        public void SetupSystem(ISystem system)
        {
            var entityChangeSubscriptions = new CompositeDisposable();
            SystemSubscriptions.Add(system, entityChangeSubscriptions);

            var castSystem = (ITeardownSystem) system;
            var affinities = castSystem.GetGroupAffinities();
            var observableGroup = ObservableGroupManager.GetObservableGroup(castSystem.Group, affinities);
            
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