using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public struct CollectionEntityEvent
    {
        public readonly IEntity Entity;
        public readonly IEntityCollection Collection;

        public CollectionEntityEvent(IEntity entity, IEntityCollection collection)
        {
            Entity = entity;
            Collection = collection;
        }
    }
}