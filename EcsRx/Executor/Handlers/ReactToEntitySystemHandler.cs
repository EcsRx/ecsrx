using System.Collections.Generic;
using System.Linq;
using System;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public class ReactToEntitySystemHandler : IReactToEntitySystemHandler
    {
        public IPoolManager PoolManager { get; private set; }

        public ReactToEntitySystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
        }

        public IEnumerable<SubscriptionToken> Setup(IReactToEntitySystem system)
        {
            var accessor = PoolManager.CreateGroupAccessor(system.TargetGroup);
            return accessor.Entities.Select(x => ProcessEntity(system, x));
        }

        public SubscriptionToken ProcessEntity(IReactToEntitySystem system, IEntity entity)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var subscription = system.ReactToEntity(entity)
                .Subscribe(x =>
                {
                    if (hasEntityPredicate)
                    {
                        var groupPredicate = system.TargetGroup as IHasPredicate;
                        if (groupPredicate.CanProcessEntity(x))
                        {  system.Execute(x); }
                        return;
                    }

                    system.Execute(x);
                });

            return new SubscriptionToken(entity, subscription);
        }
    }
}