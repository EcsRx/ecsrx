using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public struct ComponentsChangedEvent
    {
        public readonly IEntity Entity;
        public readonly int[] ComponentTypeIds;

        public ComponentsChangedEvent(IEntity entity, int[] componentTypeIds)
        {
            Entity = entity;
            ComponentTypeIds = componentTypeIds;
        }
    }
}