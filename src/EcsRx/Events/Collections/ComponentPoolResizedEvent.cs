using System;

namespace EcsRx.Events.Collections
{
    public struct ComponentPoolResizedEvent : IEquatable<ComponentPoolResizedEvent>
    {
        public readonly int ComponentTypeId;

        public ComponentPoolResizedEvent(int componentTypeId)
        {
            ComponentTypeId = componentTypeId;
        }

        public bool Equals(ComponentPoolResizedEvent other)
        {
            return ComponentTypeId == other.ComponentTypeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is ComponentPoolResizedEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ComponentTypeId;
        }
    }
}