using System;
using System.Collections.Generic;

namespace EcsRx.ReactiveData.Collections
{
    public struct CollectionAddEvent<T> : IEquatable<CollectionAddEvent<T>>
    {
        public int Index { get; }
        public T Value { get; }

        public CollectionAddEvent(int index, T value)
            :this()
        {
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return $"Index:{Index} Value:{Value}";
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(CollectionAddEvent<T> other)
        {
            return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}