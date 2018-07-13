using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Entities
{
    public class IdPool : IIdPool
    {
        public int IncrementSize => _increaseSize;
        
        private int _lastMax;
        private readonly int _increaseSize;
        
        private readonly List<int> _availableIds;

        public IdPool(int increaseSize = 1000, int startingSize = 10000)
        {
            _lastMax = startingSize;
            _increaseSize = increaseSize;
            _availableIds = Enumerable.Range(1, _lastMax).ToList();
        }

        public int Claim()
        {
            if(_availableIds.Count == 0)
            { Expand(); }
            
            var id = _availableIds[0];
            _availableIds.RemoveAt(0);

            return id;
        }

        public bool IsAvailable(int id)
        { return id > _lastMax || _availableIds.Contains(id); }

        public void ClaimSpecific(int id)
        {
            if(id > _lastMax)
            { Expand(id); }

            _availableIds.Remove(id);            
        }

        public void Free(int id)
        {
            if(id <= 0)
            { throw new ArgumentException("id has to be >= 1"); }
            
            if (id > _lastMax)
            { Expand(id); }
            
            _availableIds.Add(id);
        }

        public void Expand(int? newId = null)
        {
            var increaseBy = newId ?? _increaseSize;
            _availableIds.AddRange(Enumerable.Range(_lastMax + 1, increaseBy));
            _lastMax += increaseBy + 1;
        }
    }
}