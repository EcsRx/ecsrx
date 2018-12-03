using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public class CollectionEntityEvent
    {
        public IEntity Entity { get; }
        public IEntityCollection Collection { get; }

        public CollectionEntityEvent(IEntity entity, IEntityCollection collection)
        {
            Entity = entity;
            Collection = collection;
        }
    }
}