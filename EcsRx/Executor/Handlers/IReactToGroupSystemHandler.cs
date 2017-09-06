using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public interface IReactToGroupSystemHandler
    {
        IPoolManager PoolManager { get; }
        SubscriptionToken Setup(IReactToGroupSystem system);
    }
}