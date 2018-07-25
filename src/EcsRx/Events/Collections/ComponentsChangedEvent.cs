using System;
using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentsChangedEvent
    {
        public IEntityCollection Collection { get; }
        public IEntity Entity { get; }
        public Type[] ComponentTypes { get; }

        public ComponentsChangedEvent(IEntityCollection collection, IEntity entity, Type[] componentTypes)
        {
            Collection = collection;
            Entity = entity;
            ComponentTypes = componentTypes;
        }
    }
}