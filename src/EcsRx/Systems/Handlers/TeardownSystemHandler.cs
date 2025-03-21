using System;
using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Extensions;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.Systems;

namespace EcsRx.Systems.Handlers
{
    [Priority(10)]
    public class TeardownSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager ObservableGroupManager;
        public readonly IDictionary<ISystem, IDisposable> SystemSubscriptions;
        private readonly object _lock = new object();
        
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

            lock (_lock)
            { SystemSubscriptions.Add(system, entityChangeSubscriptions); }

            var castSystem = (ITeardownSystem) system;
            var affinities = castSystem.GetGroupAffinities();
            var observableGroup = ObservableGroupManager.GetObservableGroup(castSystem.Group, affinities);
            
            observableGroup.OnEntityRemoving
                .Subscribe(castSystem.Teardown)
                .AddTo(entityChangeSubscriptions);        
        }

        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            { SystemSubscriptions.RemoveAndDispose(system); }
        }       

        public void Dispose()
        {
            lock (_lock)
            { SystemSubscriptions.DisposeAll(); }
        }
    }
}