using System;

namespace EcsRx.Scheduling
{
    public interface IObservableScheduler : IDisposable
    {
        IObservable<TimeSpan> OnUpdate { get; }
    }
}