using System.Collections.Generic;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor
{
    public interface ISystemExecutor
    {
        IPoolManager PoolManager { get; }
        IEnumerable<ISystem> Systems { get; }

        void RemoveSystem(ISystem system);
        void AddSystem(ISystem system);
    }
}