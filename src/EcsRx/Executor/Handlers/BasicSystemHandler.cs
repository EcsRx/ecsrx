using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.MicroRx.Extensions;
using EcsRx.Scheduling;
using EcsRx.Systems;
using EcsRx.Threading;

namespace EcsRx.Executor.Handlers
{
    [Priority(6)]
    public class BasicSystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager _observableGroupManager;
        public readonly IUpdateScheduler UpdateScheduler;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        public BasicSystemHandler(IObservableGroupManager observableGroupManager, IThreadHandler threadHandler, IUpdateScheduler updateScheduler)
        {
            _observableGroupManager = observableGroupManager;
            _threadHandler = threadHandler;
            UpdateScheduler = updateScheduler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IBasicSystem; }

        public void SetupSystem(ISystem system)
        {
            var affinities = system.GetGroupAffinities();
            var observableGroup = _observableGroupManager.GetObservableGroup(system.Group, affinities);
            var hasEntityPredicate = system.Group is IHasPredicate;
            var castSystem = (IBasicSystem)system;
            var runParallel = system.ShouldMutliThread();
            IDisposable subscription;
            
            if (!hasEntityPredicate)
            {
                subscription = UpdateScheduler.OnUpdate
                    .Subscribe(x => ExecuteForGroup(observableGroup, castSystem, runParallel));
            }
            else
            {
                var groupPredicate = system.Group as IHasPredicate;
                subscription = UpdateScheduler.OnUpdate
                    .Subscribe(x => ExecuteForGroup(observableGroup
                        .Where(groupPredicate.CanProcessEntity).ToList(), castSystem, runParallel));
            }

            _systemSubscriptions.Add(system, subscription);
        }

        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IBasicSystem castSystem, bool runParallel = false)
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
        
        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}