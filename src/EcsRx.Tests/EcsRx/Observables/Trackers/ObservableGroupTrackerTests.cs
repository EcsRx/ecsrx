using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Groups.Observable.Tracking;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Observables
{
    public class CollectionObservableGroupTrackerTests
    {
        [Fact]
        public void should_raise_event_for_notify_addition_when_added_entity_matches()
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
        public void should_raise_event_for_notify_addition_when_added_entity_matches_but()
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
        public void should_not_raise_event_for_notify_addition_when_added_entity_doesnt_match()
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
    }
}