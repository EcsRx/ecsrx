using System;
using R3;
using EcsRx.Groups.Observable.Tracking.Events;

namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public interface IObservableGroupTracker : IDisposable
    {
        Observable<EntityGroupStateChanged> GroupMatchingChanged { get; }
    }
}