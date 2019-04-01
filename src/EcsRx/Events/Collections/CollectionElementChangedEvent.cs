using System;
using System.Collections.Generic;

namespace EcsRx.Events.Collections
{
    public struct CollectionElementChangedEvent<T> : IEquatable<CollectionElementChangedEvent<T>>
    {
        public int Index;
        public T OldValue;
        public T NewValue;

        public bool Equals(CollectionElementChangedEvent<T> other)
        {
            return Index == other.Index && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            return obj is CollectionElementChangedEvent<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Index;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(OldValue);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(NewValue);
                return hashCode;
            }
        }
    }
}