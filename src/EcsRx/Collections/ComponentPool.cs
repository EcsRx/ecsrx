using System;
using System.Collections;
using System.Runtime.InteropServices;
using EcsRx.Components;
using EcsRx.MicroRx.Subjects;
using EcsRx.Pools;

namespace EcsRx.Collections
{
    public unsafe class UnmanagedComponentPool<T> : IComponentPool<T>
        where T : unmanaged, IComponent
    {
        public IndexPool IndexPool { get; }
        private byte[] _components;

        public int Count { get; private set; }
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        public int ExpansionSize { get; private set; }

        public IObservable<bool> OnPoolExtending => _onPoolExtending;

        public Span<T> Components => MemoryMarshal.Cast<byte, T>(new Span<byte>(_components, 0, Count * sizeof(T)));

        private readonly Subject<bool> _onPoolExtending;

        public UnmanagedComponentPool(int expansionSize) : this(expansionSize, expansionSize)
        { }

        public UnmanagedComponentPool(int expansionSize, int initialSize)
        {
            Count = initialSize;
            ExpansionSize = expansionSize;
            IndexPool = new IndexPool(expansionSize, initialSize);
            _components = new byte[sizeof(T) * initialSize];
            _onPoolExtending = new Subject<bool>();
        }

        public int Allocate()
        {
            if (IndexesRemaining == 0)
            { Expand(); }
            return IndexPool.AllocateInstance();
        }

        public void Release(int index) => IndexPool.ReleaseInstance(index);

        public void Set(int index, object value) => _components.SetValue(value, index);

        public void Expand()
        { Expand(ExpansionSize); }

        public void Expand(int amountToAdd)
        {
            var newCount = _components.Length + amountToAdd;
            var newEntries = new byte[newCount * sizeof(T)];
            _components.CopyTo(newEntries, 0);
            IndexPool.Expand(newCount - 1);
            _components = newEntries;
            Count = newCount;

            _onPoolExtending.OnNext(true);
        }

        public void Dispose()
        { _onPoolExtending?.Dispose(); }

        public GCHandle Pin()
        {
            return GCHandle.Alloc(_components, GCHandleType.Pinned);
        }
    }

    public class ComponentPool<T> : IComponentPool<T>
        where T : IComponent
    {
        public IndexPool IndexPool { get; }
        public T[] _components;
        
        public int Count { get; private set; }
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;
        public int ExpansionSize { get; private set; }
        
        public IObservable<bool> OnPoolExtending => _onPoolExtending;

        public Span<T> Components => new Span<T>(_components, 0, Count);

        private readonly Subject<bool> _onPoolExtending;

        public ComponentPool(int expansionSize) : this(expansionSize, expansionSize)
        {}
        
        public ComponentPool(int expansionSize, int initialSize)
        {
            Count = initialSize;
            ExpansionSize = expansionSize;
            IndexPool = new IndexPool(expansionSize, initialSize);
            _components = new T[initialSize];
            _onPoolExtending = new Subject<bool>();
        }

        public int Allocate()
        {
            if(IndexesRemaining == 0) 
            { Expand(); }
            return IndexPool.AllocateInstance();
        }

        public void Release(int index) => IndexPool.ReleaseInstance(index);
        
        public void Set(int index, object value) => _components.SetValue(value, index);

        public void Expand()
        { Expand(ExpansionSize); }
        
        public void Expand(int amountToAdd)
        {
            var newCount = _components.Length + amountToAdd;
            var newEntries = new T[newCount];            
            _components.CopyTo(newEntries, 0);
            IndexPool.Expand(newCount-1);
            _components = newEntries;
            Count = newCount;
            
            _onPoolExtending.OnNext(true);
        }

        public void Dispose()
        { _onPoolExtending?.Dispose(); }

        public GCHandle Pin()
        {
            throw new NotImplementedException();
        }
    }
}