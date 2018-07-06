using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class EntityBeforeAddedEvent
    {
        public IEntity Entity { get; }
        public IEntityCollection EntityCollection { get; }

        public EntityBeforeAddedEvent(IEntity entity, IEntityCollection entityCollection)
        {
            Entity = entity;
            EntityCollection = entityCollection;
        }
    }
}