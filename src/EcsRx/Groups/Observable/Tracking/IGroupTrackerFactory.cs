using System;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        IObservableGroupIndividualTracker TrackGroup(IEntity entity, IGroup group);
        IObservableGroupIndividualTracker TrackGroup(IEntity entity, LookupGroup group);
        IObservableGroupBatchTracker TrackGroup(IGroup group);
        IObservableGroupBatchTracker TrackGroup(LookupGroup group);
    }
}