using System;
using System.Threading;

namespace EcsRx.Polyfills
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
                return new Subscribe_<T>(onError, onCompleted);
            }
            else
            {
                return new Subscribe<T>(onNext, onError, onCompleted);
            }
        }
    }

    class Subscribe<T> : IObserver<T>
    {
        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        int isStopped = 0;

        public Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
            if (isStopped == 0)
            {
                onNext(value);
            }
        }

        public void OnError(Exception error)
        {
            if (Interlocked.Increment(ref isStopped) == 1)
            {
                onError(error);
            }
        }


        public void OnCompleted()
        {
            if (Interlocked.Increment(ref isStopped) == 1)
            {
                onCompleted();
            }
        }
    }

    class Subscribe_<T> : IObserver<T>
    {
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        int isStopped = 0;

        public Subscribe_(Action<Exception> onError, Action onCompleted)
        {
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
        }

        public void OnError(Exception error)
        {
            if (Interlocked.Increment(ref isStopped) == 1)
            {
                onError(error);
            }
        }

        public void OnCompleted()
        {
            if (Interlocked.Increment(ref isStopped) == 1)
            {
                onCompleted();
            }
        }
    }
}