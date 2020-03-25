using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.MicroRx.Extensions;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;
using EcsRx.Threading;

namespace EcsRx.Plugins.ReactiveSystems.Handlers
{
    [Priority(6)]
    public class ReactToGroupSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager _observableGroupManager;       
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
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
            var affinities = system.GetGroupAffinities();
            var observableGroup = _observableGroupManager.GetObservableGroup(system.Group, affinities);
            var groupPredicate = system.Group as IHasPredicate;
            var isExtendedSystem = system is IReactToGroupExSystem;
            var castSystem = (IReactToGroupSystem)system;
            var reactObservable = castSystem.ReactToGroup(observableGroup);
            var runParallel = system.ShouldMutliThread();
                
            if (groupPredicate == null)
            {
                IDisposable noPredicateSub;

                if (isExtendedSystem)
                { noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x, (IReactToGroupExSystem)castSystem, runParallel)); }
                else
                { noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x, castSystem, runParallel)); }
                
                _systemSubscriptions.Add(system, noPredicateSub);
                return;
            }

            
            IDisposable predicateSub;
            
            if (isExtendedSystem)
            { predicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Where(groupPredicate.CanProcessEntity).ToList(), (IReactToGroupExSystem)castSystem, runParallel)); }
            else
            { predicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Where(groupPredicate.CanProcessEntity).ToList(), castSystem, runParallel)); }
            
            _systemSubscriptions.Add(system, predicateSub);
        }

        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IReactToGroupSystem castSystem, bool runParallel = false)
        {
            if (runParallel)
            {
                _threadHandler.For(0, entities.Count, i =>
                { castSystem.Process(entities[i]); });
                return;
            }
            
            for (var i = entities.Count - 1; i >= 0; i--)
            { castSystem.Process(entities[i]); }
        }
        
        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IReactToGroupExSystem castSystem, bool runParallel = false)
        {
            castSystem.BeforeProcessing();
            ExecuteForGroup(entities, (IReactToGroupSystem)castSystem, runParallel);
            castSystem.AfterProcessing();
        }
        

        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}
