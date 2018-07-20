using System;

namespace EcsRx.Computed
{
    public interface IComputed<T> : IObservable<T>
    {        
        T Value { get; }
    }
}