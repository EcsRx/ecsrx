using System.Collections.Generic;

namespace EcsRx.Lookups
{
    public interface ILookupList<TK, TV> : IReadOnlyList<TV>
    {
        void Add(TK key, TV value);
        bool ContainsKey(TK key);
        bool Remove(TK key);
        bool TryGetValue(TK key, out TV value);
        void Clear();
    }
}