using System;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupTracker : IDisposable
    {
        IObservable<GroupActionTypes> OnGroupMatchingChanged { get; }
    }
}