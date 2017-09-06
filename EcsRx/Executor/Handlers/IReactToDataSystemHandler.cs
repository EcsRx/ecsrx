using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public interface IReactToDataSystemHandler
    {
        IPoolManager PoolManager { get; }
        IEnumerable<SubscriptionToken> SetupWithoutType(ISystem system);
        SubscriptionToken ProcessEntityWithoutType(ISystem system, IEntity entity);
        IEnumerable<SubscriptionToken> Setup<T>(IReactToDataSystem<T> system);
        SubscriptionToken ProcessEntity<T>(IReactToDataSystem<T> system, IEntity entity);
    }
}