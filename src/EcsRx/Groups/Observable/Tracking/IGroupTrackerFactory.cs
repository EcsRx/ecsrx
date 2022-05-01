using System.Collections.Generic;
using EcsRx.Collections.Entity;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        IObservableGroupCollectionTracker TrackGroup(IGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingEntityComponentChanges> notifyingEntityComponentChanges);
        IObservableGroupCollectionTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingEntityComponentChanges> notifyingEntityComponentChanges);
        IObservableGroupIndividualTracker TrackGroup(IEntity entity, IGroup group);
        IObservableGroupIndividualTracker TrackGroup(IEntity entity, LookupGroup group);
        IObservableGroupBatchTracker TrackGroup(IGroup group);
        IObservableGroupBatchTracker TrackGroup(LookupGroup group);
    }
}