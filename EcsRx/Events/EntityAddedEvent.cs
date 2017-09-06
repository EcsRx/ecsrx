using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Events
{
    public class EntityAddedEvent
    {
        public IEntity Entity { get; private set; }
        public IPool Pool { get; private set; }

        public EntityAddedEvent(IEntity entity, IPool pool)
        {
            Entity = entity;
            Pool = pool;
        }
    }
}