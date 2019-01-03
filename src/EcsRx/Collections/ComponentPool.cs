using System;
using System.Collections;
using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Pools;

namespace EcsRx.Collections
{
    public struct ComponentPool : IComponentPool
    {
        public IndexPool IndexPool { get; }
        public Array Data { get; private set; }
        public int Count { get; private set; }
        
        public int IndexesRemaining => IndexPool.AvailableIndexes.Count;

        public ComponentPool(Type type, int expansionSize, int initialSize)
        {
            Count = initialSize;
            IndexPool = new IndexPool(expansionSize, initialSize);
            Data = CreateArrayFor(type, initialSize);
        }

        public static Array CreateArrayFor(Type type, int size)
        {
            var dynamicArray = type.MakeArrayType();
            return (Array)Activator.CreateInstance(dynamicArray, size);
        }
        
        public IReadOnlyList<T> AsReadOnly<T>() where T : IComponent => (IReadOnlyList<T>) Data;
        public T[] AsArray<T>()  where T : IComponent => (T[]) Data;

        public int Allocate() => IndexPool.AllocateInstance();
        public void Release(int index) => IndexPool.ReleaseInstance(index);
        
        public ref T GetRef<T>(int index) where T : IComponent => ref AsArray<T>()[index];
        public T Get<T>(int index) where T : IComponent => AsArray<T>()[index];
        public object Get(int index) => Data.GetValue(index);
        
        public void Set<T>(int index, T value) where T : IComponent => AsArray<T>()[index] = value;
        public void Set(int index, object value) => Data.SetValue(value, index);

        public void Expand(int amountToAdd)
        {
            var newCount = Data.Length + amountToAdd;
            var arrayType = Data.GetType();
            var newEntries = (Array)Activator.CreateInstance(arrayType, newCount);               
            Data.CopyTo(newEntries, 0);
            IndexPool.Expand(newCount-1);
            Data = newEntries;
            Count = newCount;
        }

        public IEnumerator GetEnumerator() => Data.GetEnumerator();
    }
}