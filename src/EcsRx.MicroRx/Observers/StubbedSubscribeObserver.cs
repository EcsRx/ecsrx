using System;
using System.Threading;

namespace EcsRx.MicroRx.Observers
{
    class StubbedSubscribeObserver<T> : IObserver<T>
    {
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        int isStopped = 0;

        public StubbedSubscribeObserver(Action<Exception> onError, Action onCompleted)
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