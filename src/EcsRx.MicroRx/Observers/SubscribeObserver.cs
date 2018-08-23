using System;
using System.Threading;

namespace EcsRx.MicroRx.Observers
{
    public class SubscribeObserver<T> : IObserver<T>
    {
        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        int isStopped = 0;

        public SubscribeObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
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
}