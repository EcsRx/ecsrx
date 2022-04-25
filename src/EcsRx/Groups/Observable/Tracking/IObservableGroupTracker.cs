using System;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IObservableGroupTracker
    {
        IObservable<GroupMatchingState> OnGroupMatchingChanged(IEntity entity, IGroup group);
        IObservable<GroupMatchingState> OnGroupMatchingChanged(IEntity entity, LookupGroup group);
    }
}