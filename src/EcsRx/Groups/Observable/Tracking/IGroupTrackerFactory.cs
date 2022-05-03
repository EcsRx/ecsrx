using System.Collections.Generic;
using EcsRx.Collections.Entity;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable.Tracking
{
    public interface IGroupTrackerFactory
    {
        ICollectionObservableGroupTracker TrackGroup(IGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges);
        ICollectionObservableGroupTracker TrackGroup(LookupGroup group, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges);
        IIndividualObservableGroupTracker TrackGroup(IEntity entity, IGroup group);
        IIndividualObservableGroupTracker TrackGroup(IEntity entity, LookupGroup group);
        IBatchObservableGroupTracker TrackGroup(IGroup group);
        IBatchObservableGroupTracker TrackGroup(LookupGroup group);
    }
}