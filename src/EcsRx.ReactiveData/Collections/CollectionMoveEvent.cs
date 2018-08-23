using System;
using System.Collections.Generic;

namespace EcsRx.Reactive.Collections
{
    public struct CollectionMoveEvent<T> : IEquatable<CollectionMoveEvent<T>>
    {
        public int OldIndex { get; }
        public int NewIndex { get; }
        public T Value { get; }

        public CollectionMoveEvent(int oldIndex, int newIndex, T value)
            : this()
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Value = value;
        }

        public override string ToString()
        {
            return $"OldIndex:{OldIndex} NewIndex:{NewIndex} Value:{Value}";
        }

        public override int GetHashCode()
        {
            return OldIndex.GetHashCode() ^ NewIndex.GetHashCode() << 2 ^ EqualityComparer<T>.Default.GetHashCode(Value) >> 2;
        }

        public bool Equals(CollectionMoveEvent<T> other)
        {
            return OldIndex.Equals(other.OldIndex) && NewIndex.Equals(other.NewIndex) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}