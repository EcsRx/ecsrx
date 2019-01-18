using System.Collections.Generic;

namespace EcsRx.ReactiveData.Dictionaries
{
    public interface IReactiveDictionary<TKey, TValue> : IReadOnlyReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
    }
}