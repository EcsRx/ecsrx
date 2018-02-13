using EcsRx.Pools;

namespace EcsRx.Events
{
    public class PoolRemovedEvent
    {
        public IPool Pool { get; private set; }

        public PoolRemovedEvent(IPool pool)
        {
            Pool = pool;
        }
    }
}