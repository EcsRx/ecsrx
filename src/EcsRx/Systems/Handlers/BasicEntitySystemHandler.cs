using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Extensions;
using SystemsRx.Scheduling;
using SystemsRx.Systems;
using SystemsRx.Threading;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using SystemsRx.MicroRx.Extensions;

namespace EcsRx.Systems.Handlers
{
    [Priority(6)]
    public class BasicEntitySystemHandler : IConventionalSystemHandler
    {
        public readonly IObservableGroupManager _observableGroupManager;
        public readonly IUpdateScheduler UpdateScheduler;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        public readonly IThreadHandler _threadHandler;
        
        public BasicEntitySystemHandler(IObservableGroupManager observableGroupManager, IThreadHandler threadHandler, IUpdateScheduler updateScheduler)
        {
            _observableGroupManager = observableGroupManager;
            _threadHandler = threadHandler;
            UpdateScheduler = updateScheduler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IBasicEntitySystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IBasicEntitySystem)system;

            var affinities = castSystem.GetGroupAffinities();
            var observableGroup = _observableGroupManager.GetObservableGroup(castSystem.Group, affinities);
            var hasEntityPredicate = castSystem.Group is IHasPredicate;
            var runParallel = system.ShouldMutliThread();
            IDisposable subscription;
            
            if (!hasEntityPredicate)
            {
                subscription = UpdateScheduler.OnUpdate
                    .Subscribe(x => ExecuteForGroup(observableGroup, castSystem, runParallel));
            }
            else
            {
                var groupPredicate = castSystem.Group as IHasPredicate;
                subscription = UpdateScheduler.OnUpdate
                    .Subscribe(x => ExecuteForGroup(observableGroup
                        .Where(groupPredicate.CanProcessEntity).ToList(), castSystem, runParallel));
            }

            _systemSubscriptions.Add(system, subscription);
        }

        private void ExecuteForGroup(IReadOnlyList<IEntity> entities, IBasicEntitySystem castSystem, bool runParallel = false)
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