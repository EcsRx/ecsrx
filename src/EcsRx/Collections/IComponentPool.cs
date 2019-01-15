using System;
using System.Collections;
using EcsRx.Components;

namespace EcsRx.Collections
{
    public interface IComponentPool<out T> : IComponentPool, IDisposable
        where T : IComponent
    {
        T[] Components { get; }
    }
    
    public interface IComponentPool : IEnumerable
    {
        int Count { get; }
        int IndexesRemaining { get; }
        
        IObservable<bool> OnPoolExtending { get; }
        
        void Expand(int amountToAdd);
        void Set(int index, object value);
        
        int Allocate();
        void Release(int index);
    }
}