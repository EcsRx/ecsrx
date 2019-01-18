using System;

namespace EcsRx.ReactiveData.Properties
{
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>
    {
        T Value { get; }
        bool HasValue { get; }
    }
}