using System;

namespace EcsRx.Infrastructure.Scheduling
{
    public interface IObservableScheduler : IDisposable
    {
        IObservable<TimeSpan> OnUpdate { get; }
    }
}