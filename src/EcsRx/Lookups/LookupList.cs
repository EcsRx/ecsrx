using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Pools;

namespace EcsRx.Lookups
{
    public class LookupList<TK, TV> : ILookupList<TK, TV>
    {
        public readonly Dictionary<TK, int> Lookups = new Dictionary<TK, int>();
        public readonly List<TV> InternalList = new List<TV>();
        public int LastIndex { get; private set; }
        
        public IEnumerator<TV> GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => Lookups.Count;
        public bool ContainsKey(TK key) => Lookups.ContainsKey(key);      
        public TV this[int index] => InternalList[index];

        public ICollection<TK> Keys => Lookups.Keys;
        public ICollection<TV> Values => InternalList;

        public TV GetByKey(TK key) => InternalList[Lookups[key]];

        public void Add(TK key, TV value)
        {
            Lookups.Add(key, LastIndex++);
            InternalList.Add(value);
        }

        public bool Remove(TK key)
        {
            if (!Lookups.ContainsKey(key)) 
            { return false;}

            LastIndex--;
            var index = Lookups[key];
            InternalList.RemoveAt(index);
            Lookups.Remove(key);
            CompactDictionaryValuesFrom(index);
            return true;
        }

        public void CompactDictionaryValuesFrom(int startingIndex)
        {
            foreach (var key in Keys.Skip(startingIndex).ToArray())
            { Lookups[key]--; }
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

        public void Clear()
        {
            InternalList.Clear();
            Lookups.Clear();
        }
    }
}