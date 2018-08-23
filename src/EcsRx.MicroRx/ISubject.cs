using System;

namespace EcsRx.MicroRx
{
    public interface ISubject<TSource, TResult> : IObserver<TSource>, IObservable<TResult>
    {
    }
    
    public interface ISubject<T> : ISubject<T, T>, IObserver<T>, IObservable<T>
    {
    }
}