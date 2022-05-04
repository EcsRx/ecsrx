using System;
using System.Collections.Generic;
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
        public void should_raise_leaving_and_left_event_for_notify_removal_when_removed_entity_matches()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(true);

            var entityRemovedSub = new Subject<CollectionEntityEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(entityRemovedSub);
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());

            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[applicableEntity.Id] = GroupMatchingType.MatchesNoExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData.Add(x);
            });
            
            entityRemovedSub.OnNext(new CollectionEntityEvent(applicableEntity));
            
            Assert.Equal(2, actualEventData.Count);
            Assert.Equal(applicableEntity, actualEventData[0].Entity);
            Assert.Equal(GroupActionType.LeavingGroup, actualEventData[0].GroupActionType);
            Assert.Equal(applicableEntity, actualEventData[1].Entity);
            Assert.Equal(GroupActionType.LeftGroup, actualEventData[1].GroupActionType);
        }
        
        [Fact]
        public void should_not_raise_leaving_or_left_event_for_notify_removal_when_removed_entity_didnt_match()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());
            
            var unapplicableEntity = Substitute.For<IEntity>();
            unapplicableEntity.Id.Returns(2);
            unapplicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(false);

            var entityRemovedSub = new Subject<CollectionEntityEvent>();
            var mockCollectionNotifier = Substitute.For<INotifyingCollection>();
            mockCollectionNotifier.EntityAdded.Returns(Observable.Empty<CollectionEntityEvent>());
            mockCollectionNotifier.EntityRemoved.Returns(entityRemovedSub);
            mockCollectionNotifier.EntityComponentsAdded.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoving.Returns(Observable.Empty<ComponentsChangedEvent>());
            mockCollectionNotifier.EntityComponentsRemoved.Returns(Observable.Empty<ComponentsChangedEvent>());
            
            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new CollectionObservableGroupTracker(lookupGroup, Array.Empty<IEntity>(), new [] {mockCollectionNotifier});
            groupTracker.EntityIdMatchTypes[unapplicableEntity.Id] = GroupMatchingType.NoMatchesNoExcludes;
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData.Add(x);
            });
            
            entityRemovedSub.OnNext(new CollectionEntityEvent(unapplicableEntity));
            
            Assert.Empty(actualEventData);
        }
    }
}