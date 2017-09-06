using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public interface ISetupSystemHandler
    {
        IPoolManager PoolManager { get; }
        IEnumerable<SubscriptionToken> Setup(ISetupSystem system);
        SubscriptionToken ProcessEntity(ISetupSystem system, IEntity entity);
    }
}