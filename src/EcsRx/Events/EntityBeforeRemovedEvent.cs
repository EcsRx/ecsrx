using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class EntityBeforeRemovedEvent
    {
        public IEntity Entity { get; }
        public IEntityCollection EntityCollection { get; }

        public EntityBeforeRemovedEvent(IEntity entity, IEntityCollection entityCollection)
        {
            Entity = entity;
            EntityCollection = entityCollection;
        }
    }
}