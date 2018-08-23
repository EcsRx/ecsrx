using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

/*
 *    This code was taken from UniRx project by neuecc
 *    https://github.com/neuecc/UniRx
 */
namespace EcsRx.Reactive.Properties
{
    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    [Serializable]
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable
    {
        static readonly IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;

        [NonSerialized]
        bool canPublishValueOnSubscribe = false;

        [NonSerialized]
        bool isDisposed = false;

        T value = default(T);

        [NonSerialized]
        Subject<T> publisher = null;

        [NonSerialized]
        IDisposable sourceConnection = null;

        [NonSerialized]
        Exception lastException = null;

        protected virtual IEqualityComparer<T> EqualityComparer => defaultEqualityComparer;

        public T Value
        {
            get => value;
            set
            {
                if (!canPublishValueOnSubscribe)
                {
                    canPublishValueOnSubscribe = true;
                    SetValue(value);

                    if (isDisposed) return; // don't notify but set value
                    var p = publisher;
                    if (p != null)
                    {
                        p.OnNext(this.value);
                    }
                    return;
                }

                if (!EqualityComparer.Equals(this.value, value))
                {
                    SetValue(value);

                    if (isDisposed) return;
                    var p = publisher;
                    if (p != null)
                    {
                        p.OnNext(this.value);
                    }
                }
            }
        }

        public bool HasValue => canPublishValueOnSubscribe;

        public ReactiveProperty()
            : this(default(T))
        {
            // default constructor 'can' publish value on subscribe.
            // because sometimes value is deserialized from UnityEngine.
        }

        public ReactiveProperty(T initialValue)
        {
            SetValue(initialValue);
            canPublishValueOnSubscribe = true;
        }

        public ReactiveProperty(IObservable<T> source)
        {
            // initialized from source's ReactiveProperty `doesn't` publish value on subscribe.
            // because there ReactiveProeprty is `Future/Task/Promise`.

            canPublishValueOnSubscribe = false;
            sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
        }

        public ReactiveProperty(IObservable<T> source, T initialValue)
        {
            canPublishValueOnSubscribe = false;
            Value = initialValue; // Value set canPublishValueOnSubcribe = true
            sourceConnection = source.Subscribe(new ReactivePropertyObserver(this));
        }

        protected virtual void SetValue(T value)
        {
            this.value = value;
        }

        public void SetValueAndForceNotify(T value)
        {
            SetValue(value);

            if (isDisposed) return;

            var p = publisher;
            if (p != null)
            {
                p.OnNext(this.value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (lastException != null)
            {
                observer.OnError(lastException);
                return Disposable.Empty;
            }

            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (publisher == null)
            {
                // Interlocked.CompareExchange is bit slower, guarantee threasafety is overkill.
                // System.Threading.Interlocked.CompareExchange(ref publisher, new Subject<T>(), null);
                publisher = new Subject<T>();
            }

            var p = publisher;
            if (p != null)
            {
                var subscription = p.Subscribe(observer);
                if (canPublishValueOnSubscribe)
                {
                    observer.OnNext(value); // raise latest value on subscribe
                }
                return subscription;
            }
            else
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
                var sc = sourceConnection;
                if (sc != null)
                {
                    sc.Dispose();
                    sourceConnection = null;
                }
                var p = publisher;
                if (p != null)
                {
                    // when dispose, notify OnCompleted
                    try
                    {
                        p.OnCompleted();
                    }
                    finally
                    {
                        p.Dispose();
                        publisher = null;
                    }
                }
            }
        }

        public override string ToString()
        {
            return (value == null) ? "(null)" : value.ToString();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        class ReactivePropertyObserver : IObserver<T>
        {
            readonly ReactiveProperty<T> parent;
            int isStopped = 0;

            public ReactivePropertyObserver(ReactiveProperty<T> parent)
            {
                this.parent = parent;
            }

            public void OnNext(T value)
            {
                parent.Value = value;
            }

            public void OnError(Exception error)
            {
                if (System.Threading.Interlocked.Increment(ref isStopped) == 1)
                {
                    parent.lastException = error;
                    var p = parent.publisher;
                    if (p != null)
                    {
                        p.OnError(error);
                    }
                    parent.Dispose(); // complete subscription
                }
            }

            public void OnCompleted()
            {
                if (System.Threading.Interlocked.Increment(ref isStopped) == 1)
                {
                    // source was completed but can publish from .Value yet.
                    var sc = parent.sourceConnection;
                    parent.sourceConnection = null;
                    if (sc != null)
                    {
                        sc.Dispose();
                    }
                }
            }
        }
    }
}