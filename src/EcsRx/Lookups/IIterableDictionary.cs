using System.Collections.Generic;

namespace EcsRx.Lookups
{
    public interface IIterableDictionary<TK, TV> : IReadOnlyList<TV>
    {
        TV Get(TK key);
        void Add(TK key, TV value);
        bool ContainsKey(TK key);
        bool Remove(TK key);
        bool TryGetValue(TK key, out TV value);
    }
}