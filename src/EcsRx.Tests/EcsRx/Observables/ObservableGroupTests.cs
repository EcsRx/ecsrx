using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events.Collections;
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
        public void should_include_entity_snapshot_on_creation()
        {
            var accessorToken = new ObservableGroupToken(new[]{1}, Array.Empty<int>(), 0);

            var applicableEntity1 = Substitute.For<IEntity>();
            var applicableEntity2 = Substitute.For<IEntity>();
            var notApplicableEntity1 = Substitute.For<IEntity>();

            applicableEntity1.Id.Returns(1);
            applicableEntity2.Id.Returns(2);
            notApplicableEntity1.Id.Returns(3);
            
            applicableEntity1.HasComponent(Arg.Any<int>()).Returns(true);
            applicableEntity2.HasComponent(Arg.Any<int>()).Returns(true);
            notApplicableEntity1.HasComponent(Arg.Any<int>()).Returns(false);            
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                applicableEntity1,
                applicableEntity2,
                notApplicableEntity1
            };
            
            var groupTracker = Substitute.For<ICollectionObservableGroupTracker>();
            groupTracker.GroupMatchingChanged.Returns(Observable.Empty<EntityGroupStateChanged>());

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
            applicableEntity.HasComponent(Arg.Is<int>(x => accessorToken.LookupGroup.RequiredComponents.Contains(x))).Returns(true);
   
            var groupStateChangedSub = new Subject<EntityGroupStateChanged>();
            var groupTracker = Substitute.For<ICollectionObservableGroupTracker>();
            groupTracker.GroupMatchingChanged.Returns(groupStateChangedSub);

            var observableGroup = new ObservableGroup(accessorToken, Array.Empty<IEntity>(), groupTracker);
            var wasCalled = 0;
            observableGroup.OnEntityAdded.Subscribe(x => wasCalled++);
            
            groupStateChangedSub.OnNext(new EntityGroupStateChanged(applicableEntity, GroupActionType.JoinedGroup));
            Assert.Equal(1, observableGroup.CachedEntities.Count);
            Assert.Equal(applicableEntity, observableGroup.CachedEntities[applicableEntity.Id]);
            
            Assert.Equal(1, wasCalled);
        }
        
    }
}