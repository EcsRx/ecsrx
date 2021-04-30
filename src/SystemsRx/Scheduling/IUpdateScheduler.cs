using System;

namespace SystemsRx.Scheduling
{
    public interface IUpdateScheduler : ITimeTracker, IDisposable
    {
        IObservable<ElapsedTime> OnPreUpdate { get; }
        IObservable<ElapsedTime> OnUpdate { get; }
        IObservable<ElapsedTime> OnPostUpdate { get; }
    }
}