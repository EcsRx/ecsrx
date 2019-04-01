using System;
using EcsRx.Entities;

namespace EcsRx.Events.Collections
{
    public struct CollectionEntityEvent : IEquatable<CollectionEntityEvent>
    {
        public readonly IEntity Entity;

        public CollectionEntityEvent(IEntity entity)
        {
            Entity = entity;
        }

        public bool Equals(CollectionEntityEvent other)
        {
            return Equals(Entity, other.Entity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is CollectionEntityEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Entity != null ? Entity.GetHashCode() : 0);
        }
    }
}