using System.Collections.Generic;

namespace EcsRx.Reactive.Dictionaries
{
    public static partial class ReactiveDictionaryExtensions
    {
        public static ReactiveDictionary<TKey, TValue> ToReactiveDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return new ReactiveDictionary<TKey, TValue>(dictionary);
        }
    }
}