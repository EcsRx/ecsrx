using System.Collections;
using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Lookups
{
    public class IterableDictionary<TK, TV> : IIterableDictionary<TK, TV>
    {
        public readonly IIdPool IdPool = new IdPool(100, 1000);
        public readonly Dictionary<TK, int> Lookups = new Dictionary<TK, int>();
        public readonly List<TV> InternalList = new List<TV>();

        public IEnumerator<TV> GetEnumerator() => InternalList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => InternalList.Count;
        public bool ContainsKey(TK key) => Lookups.ContainsKey(key);      
        public TV this[int index] => InternalList[index];

        public ICollection<TK> Keys => Lookups.Keys;
        public ICollection<TV> Values => InternalList;

        public TV Get(TK key) => InternalList[Lookups[key]];

        public void Add(TK key, TV value)
        {
            var nextIndex = IdPool.AllocateInstance();
            Lookups.Add(key, nextIndex);

            if (nextIndex < InternalList.Count)
            { InternalList[nextIndex] = value; }
            else
            { InternalList.Add(value); }
        }

        public bool Remove(TK key)
        {
            if (!Lookups.ContainsKey(key)) 
            { return false;}
            
            var index = Lookups[key];
            InternalList[index] = default(TV);
            Lookups.Remove(key);
            IdPool.ReleaseInstance(index);
            return true;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            if (!Lookups.ContainsKey(key))
            {
                value = default(TV);
                return false;
            }

            value = InternalList[Lookups[key]];
            return true;
        }
    }
}