using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class EntityRemovedEvent
    {
        public IEntity Entity { get; private set; }
        public IEntityCollection EntityCollection { get; private set; }

        public EntityRemovedEvent(IEntity entity, IEntityCollection entityCollection)
        {
            Entity = entity;
            EntityCollection = entityCollection;
        }
    }
}