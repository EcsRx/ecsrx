using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups.Observable;
using EcsRx.Tests.ComputedGroups;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ComputedGroupTests
    {
        [Fact]
        public void should_populate_entity_cache_upon_creation()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var shouldContainEntity1 = new Entity(Guid.NewGuid(), mockEventSystem);
            shouldContainEntity1.AddComponent<TestComponentOne>();
            var shouldContainEntity2 = new Entity(Guid.NewGuid(), mockEventSystem);
            shouldContainEntity2.AddComponent<TestComponentOne>();

            var shouldNotContainEntity1 = new Entity(Guid.NewGuid(), mockEventSystem);
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                shouldContainEntity1,
                shouldContainEntity2,
                shouldNotContainEntity1
            };

            mockObservableGroup.GetEnumerator().Returns(dummyEntitySnapshot.GetEnumerator());
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(Observable.Empty<IEntity>());
            var cacheableobservableGroup = new TestComputedGroup(mockObservableGroup);

            Assert.Equal(2, cacheableobservableGroup.CachedEntities.Count);
            Assert.Contains(shouldContainEntity1, cacheableobservableGroup.CachedEntities.Values);
            Assert.Contains(shouldContainEntity2, cacheableobservableGroup.CachedEntities.Values);
            Assert.DoesNotContain(shouldNotContainEntity1, cacheableobservableGroup.CachedEntities.Values);
        }
        
        [Fact]
        public void should_only_add_and_fire_event_when_applicable_entity_added()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var shouldContainEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            shouldContainEntity.AddComponent<TestComponentOne>();
            var shouldNotContainEntity = new Entity(Guid.NewGuid(), mockEventSystem);

            var dummyEntitySnapshot = new List<IEntity>();
            mockObservableGroup.GetEnumerator().Returns(dummyEntitySnapshot.GetEnumerator());

            var onEntityAddedSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(onEntityAddedSubject);
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(Observable.Empty<IEntity>());
            var cacheableobservableGroup = new TestComputedGroup(mockObservableGroup);

            var firedTimes = 0;
            cacheableobservableGroup.OnEntityAdded.Subscribe(x =>
            {
                Assert.Equal(shouldContainEntity, x);
                firedTimes++;
            });
            
            onEntityAddedSubject.OnNext(shouldContainEntity);
            onEntityAddedSubject.OnNext(shouldNotContainEntity);

            Assert.Equal(1, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(1, firedTimes);
            Assert.Contains(shouldContainEntity, cacheableobservableGroup.CachedEntities.Values);
            Assert.DoesNotContain(shouldNotContainEntity, cacheableobservableGroup.CachedEntities.Values);
        }
        
        [Fact]
        public void should_only_remove_and_fire_events_when_non_applicable_entity_removed()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var shouldContainEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            shouldContainEntity.AddComponent<TestComponentOne>();

            var dummyEntitySnapshot = new List<IEntity> { shouldContainEntity };
            mockObservableGroup.GetEnumerator().Returns(x => dummyEntitySnapshot.GetEnumerator());

            var onEntityRemovingSubject = new Subject<IEntity>();
            var onEntityRemovedSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(onEntityRemovingSubject);
            mockObservableGroup.OnEntityRemoved.Returns(onEntityRemovedSubject);
                        
            var cacheableobservableGroup = new TestComputedGroup(mockObservableGroup);

            var removingFiredTimes = 0;
            cacheableobservableGroup.OnEntityRemoving.Subscribe(x =>
            {
                Assert.Equal(shouldContainEntity, x);
                removingFiredTimes++;
            });
            
            var removedFiredTimes = 0;
            cacheableobservableGroup.OnEntityRemoved.Subscribe(x =>
            {
                Assert.Equal(shouldContainEntity, x);
                removedFiredTimes++;
            });
            
            shouldContainEntity.RemoveComponent<TestComponentOne>();
            cacheableobservableGroup.RefreshEntities();
            
            Assert.Equal(0, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(1, removingFiredTimes);
            Assert.Equal(1, removedFiredTimes);
        }
    }
}