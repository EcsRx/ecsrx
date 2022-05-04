using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Groups.Observable.Tracking;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Trackers;
using EcsRx.Groups.Observable.Tracking.Types;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Observables
{
    public class ObservableGroupTests
    {
        [Fact]
        public void should_include_matching_entity_snapshot_on_creation()
        {
            var accessorToken = new ObservableGroupToken(new[]{1}, Array.Empty<int>(), 0);

            var applicableEntity1 = Substitute.For<IEntity>();
            var applicableEntity2 = Substitute.For<IEntity>();
            var notApplicableEntity1 = Substitute.For<IEntity>();

            applicableEntity1.Id.Returns(1);
            applicableEntity2.Id.Returns(2);
            notApplicableEntity1.Id.Returns(3);
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                applicableEntity1,
                applicableEntity2,
                notApplicableEntity1
            };

            var groupMatchingChanged = new Subject<EntityGroupStateChanged>();
            var groupTracker = Substitute.For<ICollectionObservableGroupTracker>();
            groupTracker.GroupMatchingChanged.Returns(groupMatchingChanged);
            groupTracker.IsMatching(Arg.Is<int>(x => x == applicableEntity1.Id)).Returns(true);
            groupTracker.IsMatching(Arg.Is<int>(x => x == applicableEntity2.Id)).Returns(true);
            groupTracker.IsMatching(Arg.Is<int>(x => x == notApplicableEntity1.Id)).Returns(false);

            var observableGroup = new ObservableGroup(accessorToken, dummyEntitySnapshot, groupTracker);
            
            Assert.Equal(2, observableGroup.CachedEntities.Count);
            Assert.Contains(applicableEntity1, observableGroup.CachedEntities);
            Assert.Contains(applicableEntity2, observableGroup.CachedEntities);
        }

        [Fact]
        public void should_add_entity_and_raise_event_when_applicable_entity_joined()
        {
            var collectionId = 1;
            var accessorToken = new ObservableGroupToken(new[] { 1,2 }, Array.Empty<int>(), collectionId);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Id.Returns(collectionId);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
   
            var groupStateChangedSub = new Subject<EntityGroupStateChanged>();
            var groupTracker = Substitute.For<ICollectionObservableGroupTracker>();
            groupTracker.GroupMatchingChanged.Returns(groupStateChangedSub);

            var wasCalled = 0;
            var observableGroup = new ObservableGroup(accessorToken, Array.Empty<IEntity>(), groupTracker);
            observableGroup.OnEntityAdded.Subscribe(x => wasCalled++);
            groupStateChangedSub.OnNext(new EntityGroupStateChanged(applicableEntity, GroupActionType.JoinedGroup));

            Assert.Equal(1, wasCalled);
            Assert.Equal(1, observableGroup.CachedEntities.Count);
            Assert.Equal(applicableEntity, observableGroup.CachedEntities[applicableEntity.Id]);
        }
        
        [Fact]
        public void should_remove_entity_and_raise_event_when_applicable_entity_removed()
        {
            var collectionId = 1;
            var accessorToken = new ObservableGroupToken(new[] { 1,2 }, Array.Empty<int>(), collectionId);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Id.Returns(collectionId);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
   
            var groupStateChangedSub = new Subject<EntityGroupStateChanged>();
            var groupTracker = Substitute.For<ICollectionObservableGroupTracker>();
            groupTracker.GroupMatchingChanged.Returns(groupStateChangedSub);
            groupTracker.IsMatching(Arg.Is<int>(x => x == applicableEntity.Id)).Returns(true);
            
            var wasCalled = 0;
            var observableGroup = new ObservableGroup(accessorToken, new [] { applicableEntity }, groupTracker);
            observableGroup.OnEntityRemoved.Subscribe(x => wasCalled++);

            groupStateChangedSub.OnNext(new EntityGroupStateChanged(applicableEntity, GroupActionType.LeftGroup));

            Assert.Equal(1, wasCalled);
            Assert.Equal(0, observableGroup.CachedEntities.Count);
        }
        
        [Fact]
        public void should_not_remove_entity_and_raise_event_when_applicable_entity_removing()
        {
            var collectionId = 1;
            var accessorToken = new ObservableGroupToken(new[] { 1,2 }, Array.Empty<int>(), collectionId);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Id.Returns(collectionId);

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
   
            var groupStateChangedSub = new Subject<EntityGroupStateChanged>();
            var groupTracker = Substitute.For<ICollectionObservableGroupTracker>();
            groupTracker.GroupMatchingChanged.Returns(groupStateChangedSub);
            groupTracker.IsMatching(Arg.Is<int>(x => x == applicableEntity.Id)).Returns(true);
            
            var wasCalled = 0;
            var observableGroup = new ObservableGroup(accessorToken, new [] { applicableEntity }, groupTracker);
            observableGroup.OnEntityRemoving.Subscribe(x => wasCalled++);

            groupStateChangedSub.OnNext(new EntityGroupStateChanged(applicableEntity, GroupActionType.LeavingGroup));

            Assert.Equal(1, wasCalled);
            Assert.Equal(1, observableGroup.CachedEntities.Count);
            Assert.Contains(applicableEntity, observableGroup.CachedEntities);
        }
    }
}