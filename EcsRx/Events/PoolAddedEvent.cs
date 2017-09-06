using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Events
{
    public class PoolAddedEvent
    {
        public IPool Pool { get; private set; }

        public PoolAddedEvent(IPool pool)
        {
            Pool = pool;
        }
    }
}