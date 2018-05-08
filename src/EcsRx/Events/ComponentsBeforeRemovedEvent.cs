using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentsBeforeRemovedEvent
    {
        public IEntity Entity { get; }
        public IComponent[] Components { get; }

        public ComponentsBeforeRemovedEvent(IEntity entity, IComponent[] components)
        {
            Entity = entity;
            Components = components;
        }
    }
}