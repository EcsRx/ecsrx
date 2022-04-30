using System;
using EcsRx.Groups.Observable.Tracking.Events;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupTracker : IDisposable
    {
        IObservable<GroupStateChanged> GroupMatchingChanged { get; }
    }
}