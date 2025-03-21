using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.Systems;
using SystemsRx.Threading;

namespace EcsRx.Systems.Handlers
{
    [Priority(6)]
    public class ReactToGroupSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager _observableGroupManager;       
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        private readonly object _lock = new object();
        
        public ReactToGroupSystemHandler(IObservableGroupManager observableGroupManager, IThreadHandler threadHandler)
        {
            _observableGroupManager = observableGroupManager;
            _threadHandler = threadHandler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToGroupExSystem || system is IReactToGroupSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IReactToGroupSystem)system;
            var affinities = castSystem.GetGroupAffinities();
            var observableGroup = _observableGroupManager.GetObservableGroup(castSystem.Group, affinities);
            var groupPredicate = castSystem.Group as IHasPredicate;
            var isExtendedSystem = system is IReactToGroupExSystem;
            var reactObservable = castSystem.ReactToGroup(observableGroup);
            var runParallel = system.ShouldMutliThread();
                
            if (groupPredicate == null)
            {
                IDisposable noPredicateSub;

                if (isExtendedSystem)
                { noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x, (IReactToGroupExSystem)castSystem, runParallel)); }
                else
                { noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x, castSystem, runParallel)); }

                lock (_lock)
                { _systemSubscriptions.Add(system, noPredicateSub); }
                
                return;
            }

            
            IDisposable predicateSub;
            
            if (isExtendedSystem)
            { predicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Where(groupPredicate.CanProcessEntity).ToList(), (IReactToGroupExSystem)castSystem, runParallel)); }
            else
            { predicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Where(groupPredicate.CanProcessEntity).ToList(), castSystem, runParallel)); }

            lock (_lock)
            { _systemSubscriptions.Add(system, predicateSub); }
        }

        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IReactToGroupSystem system, bool runParallel = false)
        {
            if (runParallel)
            {
                _threadHandler.For(0, entities.Count, i =>
                { system.Process(entities[i]); });
                return;
            }
            
            for (var i = entities.Count - 1; i >= 0; i--)
            { system.Process(entities[i]); }
        }
        
        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IReactToGroupExSystem system, bool runParallel = false)
        {
            // We manually down cast this so it doesnt recurse this method
            var castSystem = (IReactToGroupSystem)system;
            
            system.BeforeProcessing();
            ExecuteForGroup(entities, castSystem, runParallel);
            system.AfterProcessing();
        }


        public void DestroySystem(ISystem system)
        {
            lock (_lock)
            { _systemSubscriptions.RemoveAndDispose(system); }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _systemSubscriptions.Values.DisposeAll();
                _systemSubscriptions.Clear();
            }
        }
    }
}
