using System;
using EcsRx.MicroRx.Observers;

/*
 *    This code was taken from UniRx project by neuecc
 *    https://github.com/neuecc/UniRx
 */
namespace EcsRx.MicroRx.Extensions
{
    public static class IObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(CreateSubscribeObserver(onNext, Stubs.Throw, Stubs.Nop));
        }

        internal static IObserver<T> CreateSubscribeObserver<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            // need compare for avoid iOS AOT
            if (onNext == Stubs<T>.Ignore)
            {
                return new StubbedSubscribeObserver<T>(onError, onCompleted);
            }

            return new SubscribeObserver<T>(onNext, onError, onCompleted);
        }
    }
}