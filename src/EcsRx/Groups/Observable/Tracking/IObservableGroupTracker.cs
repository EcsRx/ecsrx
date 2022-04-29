using System;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupTracker : IDisposable
    {
        IObservable<GroupActionType> OnGroupMatchingChanged { get; }
    }
}