using System.Collections.Generic;

namespace EcsRx.Reactive.Dictionaries
{
    public interface IReactiveDictionary<TKey, TValue> : IReadOnlyReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
    }
}