using System;
using System.Collections.Generic;

namespace EcsRx.Reactive.Dictionaries
{
    public struct DictionaryAddEvent<TKey, TValue> : IEquatable<DictionaryAddEvent<TKey, TValue>>
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public DictionaryAddEvent(TKey key, TValue value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"Key:{Key} Value:{Value}";
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ EqualityComparer<TValue>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(DictionaryAddEvent<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
    }
}