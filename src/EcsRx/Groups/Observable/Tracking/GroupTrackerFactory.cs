using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;

namespace EcsRx.Groups.Observable.Tracking
{
    public class GroupTrackerFactory : IGroupTrackerFactory
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public GroupTrackerFactory(IComponentTypeLookup componentTypeLookup)
        { ComponentTypeLookup = componentTypeLookup; }

        public IObservableGroupIndividualTracker TrackGroup(IEntity entity, IGroup group)
        { return TrackGroup(entity, ComponentTypeLookup.GetLookupGroupFor(group)); }

        public IObservableGroupIndividualTracker TrackGroup(IEntity entity, LookupGroup group)
        { return new ObservableGroupIndividualTracker(entity, group); }

        public IObservableGroupBatchTracker TrackGroup(IGroup group)
        { return TrackGroup(ComponentTypeLookup.GetLookupGroupFor(group)); }

        public IObservableGroupBatchTracker TrackGroup(LookupGroup group)
        { return new ObservableGroupBatchTracker(group); }
    }
}