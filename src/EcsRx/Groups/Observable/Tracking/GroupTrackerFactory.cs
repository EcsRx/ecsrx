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

        public IObservableGroupTracker TrackGroup(IEntity entity, IGroup group)
        { return TrackGroup(entity, ComponentTypeLookup.GetLookupGroupFor(group)); }

        public IObservableGroupTracker TrackGroup(IEntity entity, LookupGroup group)
        { return new ObservableGroupTracker(entity, group); }

        
    }
}