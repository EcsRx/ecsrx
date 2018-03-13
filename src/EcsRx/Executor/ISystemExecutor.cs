using System.Collections.Generic;
using EcsRx.Systems;

namespace EcsRx.Executor
{
    public interface ISystemExecutor
    {
        IEnumerable<ISystem> Systems { get; }

        void RemoveSystem(ISystem system);
        void AddSystem(ISystem system);
    }
}