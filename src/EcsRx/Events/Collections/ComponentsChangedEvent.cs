using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentsChangedEvent
    {
        public IEntityCollection Collection { get; }
        public IEntity Entity { get; }
        public IComponent[] Components { get; }

        public ComponentsChangedEvent(IEntityCollection collection, IEntity entity, IComponent[] components)
        {
            Collection = collection;
            Entity = entity;
            Components = components;
        }
    }
}