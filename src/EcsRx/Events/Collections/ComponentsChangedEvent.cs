using System;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public struct ComponentsChangedEvent : IEquatable<ComponentsChangedEvent>
    {
        public readonly IEntity Entity;
        public readonly int[] ComponentTypeIds;

        public ComponentsChangedEvent(IEntity entity, int[] componentTypeIds)
        {
            Entity = entity;
            ComponentTypeIds = componentTypeIds;
        }

        public bool Equals(ComponentsChangedEvent other)
        {
            return Equals(Entity, other.Entity) && Equals(ComponentTypeIds, other.ComponentTypeIds);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is ComponentsChangedEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Entity != null ? Entity.GetHashCode() : 0) * 397) ^ (ComponentTypeIds != null ? ComponentTypeIds.GetHashCode() : 0);
            }
        }
    }
}