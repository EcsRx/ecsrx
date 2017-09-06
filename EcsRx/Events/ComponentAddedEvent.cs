using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentAddedEvent
    {
        public IEntity Entity { get; private set; }
        public IComponent Component { get; private set; }

        public ComponentAddedEvent(IEntity entity, IComponent component)
        {
            Entity = entity;
            Component = component;
        }
    }
}