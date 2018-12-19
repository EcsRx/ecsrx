using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public struct CollectionEntityEvent
    {
        public readonly IEntity Entity;

        public CollectionEntityEvent(IEntity entity)
        {
            Entity = entity;
        }
    }
}