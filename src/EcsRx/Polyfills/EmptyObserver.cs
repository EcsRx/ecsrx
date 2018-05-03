using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Polyfills
{
    public class ListObserver<T> : IObserver<T>
    {
        private readonly List<IObserver<T>> _observers;

        public ListObserver(List<IObserver<T>> observers)
        {
            _observers = observers;
        }

        public void OnCompleted()
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].OnError(error);
            }
        }

        public void OnNext(T value)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].OnNext(value);
            }
        }

        internal IObserver<T> Add(IObserver<T> observer)
        {
            _observers.Add(observer);
            return new ListObserver<T>(_observers);
        }

        internal IObserver<T> Remove(IObserver<T> observer)
        {
            var i = _observers.IndexOf(observer);
            if (i < 0)
                return this;

            if (_observers.Count == 2)
            {
                return _observers[1 - i];
            }
            else
            {
                _observers.Remove(observer);
                return new ListObserver<T>(_observers);
            }
        }
    }

    public class EmptyObserver<T> : IObserver<T>
    {
        public static readonly EmptyObserver<T> Instance = new EmptyObserver<T>();

        EmptyObserver()
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
        }
    }

    public class ThrowObserver<T> : IObserver<T>
    {
        public static readonly ThrowObserver<T> Instance = new ThrowObserver<T>();

        ThrowObserver()
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(T value)
        {
        }
    }

    public class DisposedObserver<T> : IObserver<T>
    {
        public static readonly DisposedObserver<T> Instance = new DisposedObserver<T>();

        DisposedObserver()
        {

        }

        public void OnCompleted()
        {
            throw new ObjectDisposedException("");
        }

        public void OnError(Exception error)
        {
            throw new ObjectDisposedException("");
        }

        public void OnNext(T value)
        {
            throw new ObjectDisposedException("");
        }
    }
}