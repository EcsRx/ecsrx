using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentsAddedEvent
    {
        public IEntity Entity { get; }
        public IComponent[] Components { get; }

        public ComponentsAddedEvent(IEntity entity, IComponent[] components)
        {
            Entity = entity;
            Components = components;
        }
    }
}