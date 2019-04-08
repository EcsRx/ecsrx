using System;

namespace EcsRx.ReactiveData
{
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>, IDisposable
    {
        T Value { get; }
        bool HasValue { get; }
    }
}