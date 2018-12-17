using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public struct ComponentsChangedEvent
    {
        public readonly IEntityCollection Collection;
        public readonly IEntity Entity;
        public readonly int[] ComponentTypeIds;

        public ComponentsChangedEvent(IEntityCollection collection, IEntity entity, int[] componentTypeIds)
        {
            Collection = collection;
            Entity = entity;
            ComponentTypeIds = componentTypeIds;
        }
    }
}