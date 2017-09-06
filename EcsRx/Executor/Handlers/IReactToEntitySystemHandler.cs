using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public interface IReactToEntitySystemHandler
    {
        IPoolManager PoolManager { get; }
        IEnumerable<SubscriptionToken> Setup(IReactToEntitySystem system);
        SubscriptionToken ProcessEntity(IReactToEntitySystem system, IEntity entity);
    }
}