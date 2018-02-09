using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public class ReactToDataSystemHandler : IReactToDataSystemHandler
    {
        public IPoolManager PoolManager { get; }

        public ReactToDataSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
        }

        // TODO: This is REALLY bad but currently no other way around the dynamic invocation lookup stuff
        public IEnumerable<SubscriptionToken> SetupWithoutType(ISystem system)
        {
            var method = GetType().GetMethod("Setup");
            var genericDataType = system.GetGenericDataType();
            var genericMethod = method.MakeGenericMethod(genericDataType);
            return (IEnumerable<SubscriptionToken>)genericMethod.Invoke(this, new[] { system });
        }

        // TODO: This is REALLY bad but currently no other way around the dynamic invocation lookup stuff
        public SubscriptionToken ProcessEntityWithoutType(ISystem system, IEntity entity)
        {
            var method = GetType().GetMethod("ProcessEntity");
            var genericDataType = system.GetGenericDataType();
            var genericMethod = method.MakeGenericMethod(genericDataType);
            return (SubscriptionToken)genericMethod.Invoke(this, new object[] { system, entity });
        }

        public IEnumerable<SubscriptionToken> Setup<T>(IReactToDataSystem<T> system)
        {
            var groupAccessor = PoolManager.CreateObservableGroup(system.TargetGroup);
            return groupAccessor.Entities.Select(x => ProcessEntity(system, x));
        }

        public SubscriptionToken ProcessEntity<T>(IReactToDataSystem<T> system, IEntity entity)
        {
            var hasEntityPredicate = system.TargetGroup is IHasPredicate;
            var subscription = system.ReactToData(entity)
                    .Subscribe(x =>
                    {
                        if (hasEntityPredicate)
                        {
                            var groupPredicate = system.TargetGroup as IHasPredicate;
                            if(groupPredicate.CanProcessEntity(entity))
                            { system.Execute(entity, x); }
                            return;
                        }

                        system.Execute(entity, x);
                    });

            return new SubscriptionToken(entity, subscription);
        }

        public void Dispose()
        {
        }
    }
}