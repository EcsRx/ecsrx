using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections.Entity;
using EcsRx.Collections.Events;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Trackers;
using EcsRx.Groups.Observable.Tracking.Types;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Observables.Trackers
{
    public class CollectionObservableGroupTrackerTests
    {
        [Fact]
        public void should_raise_joining_event_for_notify_addition_when_added_entity_matches()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(true);

            var entityAddedSub = new Subject<CollectionEntityEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(entityAddedSub);
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());

            var timesCalled = 0;
            var actualEventData = new EntityGroupStateChanged();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData = x;
                timesCalled++;
            });
            
            entityAddedSub.OnNext(new CollectionEntityEvent(applicableEntity));
            
            Assert.Equal(1, timesCalled);
            Assert.Equal(applicableEntity, actualEventData.Entity);
            Assert.Equal(GroupActionType.JoinedGroup, actualEventData.GroupActionType);
        }
        
        [Fact]
        public void should_not_raise_joining_event_for_notify_addition_when_added_entity_doesnt_match()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());
            
            var unapplicableEntity = Substitute.For<IEntity>();
            unapplicableEntity.Id.Returns(2);
            unapplicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(false);

            var entityAdded = new Subject<CollectionEntityEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(entityAdded);
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var timesCalled = 0;
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.GroupMatchingChanged.Subscribe(x => timesCalled++);
            
            entityAdded.OnNext(new CollectionEntityEvent(unapplicableEntity));
            
            Assert.Equal(0, timesCalled);
        }
        
        [Fact]
        public void should_raise_joining_event_for_notify_when_entity_goes_from_non_match_to_matches_via_add_component()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(true);

            var entityComponentAddedSub = new Subject<ComponentsChangedEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(entityComponentAddedSub);
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());

            var timesCalled = 0;
            var actualEventData = new EntityGroupStateChanged();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[entity.Id] = GroupMatchingType.NoMatchesNoExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData = x;
                timesCalled++;
            });
            
            entityComponentAddedSub.OnNext(new ComponentsChangedEvent(entity, lookupGroup.RequiredComponents));
            Assert.Equal(1, timesCalled);
            Assert.Equal(entity, actualEventData.Entity);
            Assert.Equal(GroupActionType.JoinedGroup, actualEventData.GroupActionType);
        }
        
        [Fact]
        public void should_raise_leaving_left_events_for_notify_when_entity_goes_from_match_to_no_matches_via_add_component()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, new[] {3});

            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);

            var entityComponentAddedSub = new Subject<ComponentsChangedEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(entityComponentAddedSub);
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());

            var timesCalled = 0;
            var actualEventData = new EntityGroupStateChanged[2];
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[entity.Id] = GroupMatchingType.MatchesNoExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData[timesCalled] = x;
                timesCalled++;
            });
            
            entityComponentAddedSub.OnNext(new ComponentsChangedEvent(entity, lookupGroup.ExcludedComponents));
            Assert.Equal(2, timesCalled);
            Assert.Equal(entity, actualEventData[0].Entity);
            Assert.Equal(entity, actualEventData[1].Entity);
            Assert.Equal(GroupActionType.LeavingGroup, actualEventData[0].GroupActionType);
            Assert.Equal(GroupActionType.LeftGroup, actualEventData[1].GroupActionType);
        }
        
        [Fact]
        public void should_raise_joining_event_for_notify_when_entity_goes_from_non_match_to_matches_via_remove_component()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(true);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(false);

            var entityComponentRemovedSub = new Subject<ComponentsChangedEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(entityComponentRemovedSub);

            var timesCalled = 0;
            var actualEventData = new EntityGroupStateChanged();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[entity.Id] = GroupMatchingType.MatchesWithExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData = x;
                timesCalled++;
            });
            
            entityComponentRemovedSub.OnNext(new ComponentsChangedEvent(entity, lookupGroup.ExcludedComponents));
            Assert.Equal(1, timesCalled);
            Assert.Equal(entity, actualEventData.Entity);
            Assert.Equal(GroupActionType.JoinedGroup, actualEventData.GroupActionType);
        }
        
        [Fact]
        public void should_raise_left_events_for_notify_when_entity_goes_from_match_to_no_matches_via_remove_component()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(false);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(false);

            var entityComponentRemovedSub = new Subject<ComponentsChangedEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(entityComponentRemovedSub);

            var timesCalled = 0;
            var actualEventData = new EntityGroupStateChanged();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[entity.Id] = GroupMatchingType.MatchesNoExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData = x;
                timesCalled++;
            });
            
            entityComponentRemovedSub.OnNext(new ComponentsChangedEvent(entity, lookupGroup.RequiredComponents));
            Assert.Equal(1, timesCalled);
            Assert.Equal(entity, actualEventData.Entity);
            Assert.Equal(GroupActionType.LeftGroup, actualEventData.GroupActionType);
        }
        
                
        [Fact]
        public void should_raise_leaving_events_for_notify_when_entity_goes_from_match_to_no_matches_via_removing_component()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(false);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(false);

            var entityComponentRemovingSub = new Subject<ComponentsChangedEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(entityComponentRemovingSub);
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());

            var timesCalled = 0;
            var actualEventData = new EntityGroupStateChanged();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[entity.Id] = GroupMatchingType.MatchesNoExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData = x;
                timesCalled++;
            });
            
            entityComponentRemovingSub.OnNext(new ComponentsChangedEvent(entity, lookupGroup.RequiredComponents));
            Assert.Equal(1, timesCalled);
            Assert.Equal(entity, actualEventData.Entity);
            Assert.Equal(GroupActionType.LeavingGroup, actualEventData.GroupActionType);
        }
    }
}