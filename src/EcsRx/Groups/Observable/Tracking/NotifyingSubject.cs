using System;
using SystemsRx.MicroRx;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking
{
    public class NotifyOnDisposeSubject<T> : ISubject<T>, IDisposable
    {
        private Subject<T> _internalSubject;
        private Subject<Unit> _internalDisposableSubject;
        public IObservable<Unit> Disposed => _internalDisposableSubject;

        public NotifyOnDisposeSubject(Subject<T> internalSubject)
        {
            _internalSubject = internalSubject;
            _internalDisposableSubject = new Subject<Unit>();
        }

        public void OnCompleted() => _internalSubject.OnCompleted();
        public void OnError(Exception error) => _internalSubject.OnError(error);
        public void OnNext(T value) => _internalSubject.OnNext(value);
        public IDisposable Subscribe(IObserver<T> observer) => _internalSubject.Subscribe(observer);

        public void Dispose()
        {
            _internalSubject.Dispose();
            _internalDisposableSubject.OnNext(Unit.Default);
            _internalDisposableSubject?.Dispose();
        }
    }
}