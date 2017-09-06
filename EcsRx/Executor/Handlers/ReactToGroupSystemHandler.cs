using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using System;
using System.Linq;

namespace EcsRx.Executor.Handlers
{
    public class ReactToGroupSystemHandler : IReactToGroupSystemHandler
    {
        public IPoolManager PoolManager { get; private set; }

        public ReactToGroupSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
        }

        public SubscriptionToken Setup(IReactToGroupSystem system)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var groupAccessor = PoolManager.CreateGroupAccessor(system.TargetGroup);
            var subscription = system.ReactToGroup(groupAccessor)
                .Subscribe(accessor =>
                {
                    var entities = accessor.Entities.ToList();
                    var entityCount = entities.Count - 1;
                    for (var i = entityCount; i >= 0; i--)
                    {
                        if (hasEntityPredicate)
                        {
                            var groupPredicate = system.TargetGroup as IHasPredicate;
                            if (groupPredicate.CanProcessEntity(entities[i]))
                            {
                                system.Execute(entities[i]);
                            }
                            continue;
                        }

                        system.Execute(entities[i]);
                    }
                });

            return new SubscriptionToken(null, subscription);
        }
    }
}
