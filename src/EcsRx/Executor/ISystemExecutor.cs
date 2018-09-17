using System.Collections.Generic;
using EcsRx.Systems;

namespace EcsRx.Executor
{
    public interface ISystemExecutor
    {
        IEnumerable<ISystem> Systems { get; }

        bool HasSystem(ISystem system);
        void RemoveSystem(ISystem system);
        void AddSystem(ISystem system);
    }
}