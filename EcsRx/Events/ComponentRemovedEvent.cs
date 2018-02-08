using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentsRemovedEvent
    {
        public IEntity Entity { get; }
        public IComponent[] Components { get; }

        public ComponentsRemovedEvent(IEntity entity, IComponent[] components)
        {
            Entity = entity;
            Components = components;
        }
    }
}