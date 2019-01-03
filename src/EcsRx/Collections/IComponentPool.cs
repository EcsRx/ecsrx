using System.Collections;
using System.Collections.Generic;
using EcsRx.Components;

namespace EcsRx.Collections
{
    public interface IComponentPool : IEnumerable
    {
        int Count { get; }
        int IndexesRemaining { get; }
        
        IReadOnlyList<T> AsReadOnly<T>() where T : IComponent; 
        T[] AsArray<T>() where T : IComponent;
        
        ref T GetRef<T>(int index) where T : IComponent;
        T Get<T>(int index) where T : IComponent;
        void Set<T>(int index, T value) where T : IComponent;
        void Expand(int amountToAdd);
        void Set(int index, object value);
        
        int Allocate();
        void Release(int index);
    }
}