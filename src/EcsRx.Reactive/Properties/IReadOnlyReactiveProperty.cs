using System;

namespace EcsRx.Reactive.Properties
{
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>
    {
        T Value { get; }
        bool HasValue { get; }
    }
}