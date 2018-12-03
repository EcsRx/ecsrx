using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Pools
{   
    public class IndexPool : IPool<int>
    {
        public int IncrementSize => _increaseSize;
        
        private int _lastMax;
        private readonly int _increaseSize;
        
        public readonly Stack<int> AvailableEntries;

        public IndexPool(int increaseSize = 100, int startingSize = 1000)
        {
            _lastMax = startingSize;
            _increaseSize = increaseSize;
            AvailableEntries = new Stack<int>(Enumerable.Range(0, _lastMax).Reverse());
        }

        public int AllocateInstance()
        {
            if(AvailableEntries.Count == 0)
            { Expand(); }
            
            return AvailableEntries.Pop();
        }

        public void ReleaseInstance(int index)
        {
            if(index < 0)
            { throw new ArgumentException("id has to be >= 0"); }
            
            if (index > _lastMax)
            { Expand(index); }
            
            AvailableEntries.Push(index);
        }

        public void Expand(int? newId = null)
        {
            var increaseBy = newId -_lastMax ?? _increaseSize;
            
            var newEntries = Enumerable.Range(_lastMax, increaseBy).Reverse();
            _lastMax += increaseBy + 1;
            
            foreach(var entry in newEntries)
            { AvailableEntries.Push(entry); }
        }
    }
}