using System.Collections;
using System.Collections.Generic;
using EcsRx.Components;

namespace EcsRx.Collections
{
    public interface IComponentPool<out T> : IComponentPool
        where T : IComponent
    {
        T[] Components { get; }
    }
    
    public interface IComponentPool : IEnumerable
    {
        int Count { get; }
        int IndexesRemaining { get; }
        
        void Expand(int amountToAdd);
        void Set(int index, object value);
        
        int Allocate();
        void Release(int index);
    }
}