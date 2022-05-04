using System.Collections.Generic;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Groups.Observable.Tracking.Trackers;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        ICollectionObservableGroupTracker TrackGroup(IGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges);
        ICollectionObservableGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges);
        IBatchObservableGroupTracker TrackGroup(IGroup group);
        IBatchObservableGroupTracker TrackGroup(LookupGroup group);
        IIndividualObservableGroupTracker TrackGroup(IGroup group, IEntity entity);
        IIndividualObservableGroupTracker TrackGroup(LookupGroup group, IEntity entity);
    }
}