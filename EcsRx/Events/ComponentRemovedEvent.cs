using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class ComponentRemovedEvent
    {
        public IEntity Entity { get; private set; }
        public IComponent Component { get; private set; }

        public ComponentRemovedEvent(IEntity entity, IComponent component)
        {
            Entity = entity;
            Component = component;
        }
    }
}