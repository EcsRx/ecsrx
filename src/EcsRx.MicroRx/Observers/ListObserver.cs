using System;

namespace EcsRx.MicroRx.Observers
{
    public class ListObserver<T> : IObserver<T>
    {
        private readonly ImmutableList<IObserver<T>> _observers;

        public ListObserver(ImmutableList<IObserver<T>> observers)
        {
            _observers = observers;
        }

        public void OnCompleted()
        {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++)
            {
                targetObservers[i].OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++)
            {
                targetObservers[i].OnError(error);
            }
        }

        public void OnNext(T value)
        {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++)
            {
                targetObservers[i].OnNext(value);
            }
        }

        internal IObserver<T> Add(IObserver<T> observer)
        {
            return new ListObserver<T>(_observers.Add(observer));
        }

        internal IObserver<T> Remove(IObserver<T> observer)
        {
            var i = Array.IndexOf(_observers.Data, observer);
            if (i < 0)
                return this;

            if (_observers.Data.Length == 2)
            {
                return _observers.Data[1 - i];
            }
            else
            {
                return new ListObserver<T>(_observers.Remove(observer));
            }
        }
    }
}