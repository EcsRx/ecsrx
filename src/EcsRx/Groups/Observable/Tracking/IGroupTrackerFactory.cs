using System;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        IObservableGroupTracker TrackGroup(IEntity entity, IGroup group);
        IObservableGroupTracker TrackGroup(IEntity entity, LookupGroup group);
    }
}