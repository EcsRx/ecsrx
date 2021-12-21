using System;
using System.Collections;
using SystemsRx.MicroRx.Subjects;
using SystemsRx.Pools;

namespace EcsRx.Components
{
    public class ComponentPool<T> : IComponentPool<T>
        where T : IComponent
    {
        public bool IsStructType { get; }
        
        public IndexPool IndexPool { get; }
        public T[] Components { get; private set; }
        
        public int Count { get; private set; }
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        public int ExpansionSize { get; private set; }
        
        public IObservable<bool> OnPoolExtending => _onPoolExtending;
        private readonly Subject<bool> _onPoolExtending;

        public ComponentPool(int expansionSize) : this(expansionSize, expansionSize)
        { }
        
        public ComponentPool(int expansionSize, int initialSize)
        {
            Count = initialSize;
            ExpansionSize = expansionSize;
            IndexPool = new IndexPool(expansionSize, initialSize);
            Components = new T[initialSize];
            _onPoolExtending = new Subject<bool>();
            IsStructType = typeof(T).IsValueType;
        }

        public int Allocate()
        {
            if(IndexesRemaining == 0) 
            { Expand(); }
            return IndexPool.AllocateInstance();
        }

        public void Release(int index)
        {
            var instance = Components[index];
            
            if(!IsStructType)
            { Components[index] = default; }
            
            if(instance is IDisposable disposable)
            { disposable.Dispose(); }
            
            IndexPool.ReleaseInstance(index);
        }

        public void Set(int index, object value) => Components.SetValue(value, index);

        public void Expand()
        { Expand(ExpansionSize); }
        
        public void Expand(int amountToAdd)
        {
            var newCount = Components.Length + amountToAdd;
            var newEntries = new T[newCount];            
            Components.CopyTo(newEntries, 0);
            IndexPool.Expand(newCount-1);
            Components = newEntries;
            Count = newCount;
            
            _onPoolExtending.OnNext(true);
        }

        public IEnumerator GetEnumerator() => Components.GetEnumerator();

        public void Dispose()
        { _onPoolExtending?.Dispose(); }
    }
}