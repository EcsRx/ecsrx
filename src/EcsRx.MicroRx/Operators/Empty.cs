using System;
using EcsRx.MicroRx.Disposables;

namespace EcsRx.MicroRx.Operators
{
    public class ImmutableEmptyObservable<T> : IObservable<T>
    {
        public static ImmutableEmptyObservable<T> Instance = new ImmutableEmptyObservable<T>();

        ImmutableEmptyObservable()
        {

        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}