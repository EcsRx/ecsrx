using System;

namespace EcsRx.Plugins.Computeds
{
    public interface IComputed<out T> : IObservable<T>
    {        
        T Value { get; }
    }
}