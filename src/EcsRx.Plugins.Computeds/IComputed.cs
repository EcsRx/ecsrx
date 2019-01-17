using System;

namespace EcsRx.Plugins.Computeds
{
    public interface IComputed<T> : IObservable<T>
    {        
        T Value { get; }
    }
}