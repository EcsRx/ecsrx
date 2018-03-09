using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using EcsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace EcsRx.Executor.Handlers
{
    [Priority(2)]
    public class ReactToGroupSystemHandler : IConventionalSystemHandler
    {
        public readonly IPoolManager _poolManager;       
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public ReactToGroupSystemHandler(IPoolManager poolManager)
        {
            _poolManager = poolManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToGroupSystem; }

        public void SetupSystem(ISystem system)
        {
            var groupAccessor = _poolManager.CreateObservableGroup(system.TargetGroup);
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var castSystem = (IReactToGroupSystem)system;
            var reactObservable = castSystem.ReactToGroup(groupAccessor);

            if (!hasEntityPredicate)
            {
                var noPredicateSub = reactObservable.Subscribe(x => ExecuteForGroup(x.Entities, castSystem));
                _systemSubscriptions.Add(system, noPredicateSub);
                return;
            }

            var groupPredicate = system.TargetGroup as IHasPredicate;
            var subscription = reactObservable.Subscribe(x => ExecuteForGroup(x.Entities.Where(groupPredicate.CanProcessEntity), castSystem));
            _systemSubscriptions.Add(system, subscription);
        }

        private static void ExecuteForGroup(IEnumerable<IEntity> entities, IReactToGroupSystem castSystem)
        {
            foreach(var entity in entities)
            { castSystem.Execute(entity); }
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
