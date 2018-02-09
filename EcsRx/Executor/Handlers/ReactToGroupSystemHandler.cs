using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using EcsRx.Extensions;

namespace EcsRx.Executor.Handlers
{
    public class ReactToGroupSystemHandler : IConventionalSystemHandler<IReactToGroupSystem>
    {
        public IPoolManager PoolManager { get; }
        
        private readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public ReactToGroupSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IReactToGroupSystem; }

        public void SetupSystem(IReactToGroupSystem system)
        {
            var groupAccessor = PoolManager.CreateObservableGroup(system.TargetGroup);
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var reactObservable = system.ReactToGroup(groupAccessor);

            if (!hasEntityPredicate)
            {
                var noPredicateSub = reactObservable.Subscribe(x => x.Entities.ForEachRun(system.Execute));
                _systemSubscriptions.Add(system, noPredicateSub);
                return;
            }

            var groupPredicate = system.TargetGroup as IHasPredicate;
            var subscription = reactObservable.Subscribe(x =>
            {
                x.Entities.Where(groupPredicate.CanProcessEntity)
                    .ForEachRun(system.Execute);
            });
            _systemSubscriptions.Add(system, subscription);
        }

        public void DestroySystem(IReactToGroupSystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}
