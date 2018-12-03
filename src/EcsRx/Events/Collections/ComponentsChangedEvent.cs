using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public class ComponentsChangedEvent
    {
        public IEntityCollection Collection { get; }
        public IEntity Entity { get; }
        public int[] ComponentTypeIds { get; }

        public ComponentsChangedEvent(IEntityCollection collection, IEntity entity, int[] componentTypeIds)
        {
            Collection = collection;
            Entity = entity;
            ComponentTypeIds = componentTypeIds;
        }
    }
}