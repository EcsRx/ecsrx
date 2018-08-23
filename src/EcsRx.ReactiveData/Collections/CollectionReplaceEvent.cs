using System;
using System.Collections.Generic;

namespace EcsRx.Reactive.Collections
{
    public struct CollectionReplaceEvent<T> : IEquatable<CollectionReplaceEvent<T>>
    {
        public int Index { get; }
        public T OldValue { get; }
        public T NewValue { get; }

        public CollectionReplaceEvent(int index, T oldValue, T newValue)
            : this()
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            return $"Index:{Index} OldValue:{OldValue} NewValue:{NewValue}";
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(OldValue) << 2 ^ EqualityComparer<T>.Default.GetHashCode(NewValue) >> 2;
        }

        public bool Equals(CollectionReplaceEvent<T> other)
        {
            return Index.Equals(other.Index)
                   && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue)
                   && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
        }
    }
}