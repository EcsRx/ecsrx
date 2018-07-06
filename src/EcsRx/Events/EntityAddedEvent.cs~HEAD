using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class EntityAddedEvent
    {
        public IEntity Entity { get; }
        public IEntityCollection EntityCollection { get; }

        public EntityAddedEvent(IEntity entity, IEntityCollection entityCollection)
        {
            Entity = entity;
            EntityCollection = entityCollection;
        }
    }
}