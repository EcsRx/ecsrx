using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentsBeforeAddedEvent
    {
        public IEntity Entity { get; }
        public IComponent[] Components { get; }

        public ComponentsBeforeAddedEvent(IEntity entity, IComponent[] components)
        {
            Entity = entity;
            Components = components;
        }
    }
}