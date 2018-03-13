using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class EntityAddedEvent
    {
        public IEntity Entity { get; private set; }
        public IEntityCollection EntityCollection { get; private set; }

        public EntityAddedEvent(IEntity entity, IEntityCollection entityCollection)
        {
            Entity = entity;
            EntityCollection = entityCollection;
        }
    }
}