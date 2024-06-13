using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Tests.EcsRx.Computeds.Models;
using EcsRx.Tests.Models;
using NSubstitute;
using R3;
using Xunit;

namespace EcsRx.Tests.EcsRx.Computeds
{
    public class ComputedGroupTests
    {
        
        [Fact]
        public void should_populate_entity_cache_upon_creation()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var shouldContainEntity1 = Substitute.For<IEntity>();
            shouldContainEntity1.Id.Returns(1);
            shouldContainEntity1.HasComponent<TestComponentOne>().Returns(true);
            
            var shouldContainEntity2 = Substitute.For<IEntity>();
            shouldContainEntity1.Id.Returns(2);
            shouldContainEntity2.HasComponent<TestComponentOne>().Returns(true);
            
            var shouldNotContainEntity1 = Substitute.For<IEntity>();
            shouldContainEntity1.Id.Returns(3);
            shouldNotContainEntity1.HasComponent<TestComponentOne>().Returns(false);
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                shouldContainEntity1,
                shouldContainEntity2,
                shouldNotContainEntity1
            };

            mockObservableGroup.GetEnumerator().Returns(x => dummyEntitySnapshot.GetEnumerator());
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(Observable.Empty<IEntity>());
            var computedGroup = new TestComputedGroup(mockObservableGroup);

            Assert.Equal(2, computedGroup.CachedEntities.Count);
            Assert.Contains(shouldContainEntity1, computedGroup.CachedEntities);
            Assert.Contains(shouldContainEntity2, computedGroup.CachedEntities);
            Assert.DoesNotContain(shouldNotContainEntity1, computedGroup.CachedEntities);
        }
        
        [Fact]
        public void should_only_add_and_fire_event_when_applicable_entity_added()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var shouldContainEntity = Substitute.For<IEntity>();
            shouldContainEntity.Id.Returns(1);
            shouldContainEntity.HasComponent<TestComponentOne>().Returns(true);
            
            var shouldNotContainEntity = Substitute.For<IEntity>();
            shouldContainEntity.Id.Returns(2);
            shouldNotContainEntity.HasComponent<TestComponentOne>().Returns(false);

            var dummyEntitySnapshot = new List<IEntity>();
            mockObservableGroup.GetEnumerator().Returns(x => dummyEntitySnapshot.GetEnumerator());

            var onEntityAddedSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(onEntityAddedSubject);
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(Observable.Empty<IEntity>());
            var computedGroup = new TestComputedGroup(mockObservableGroup);

            var firedTimes = 0;
            computedGroup.OnEntityAdded.Subscribe(x =>
            {
                Assert.Equal(shouldContainEntity, x);
                firedTimes++;
            });
            
            onEntityAddedSubject.OnNext(shouldContainEntity);
            onEntityAddedSubject.OnNext(shouldNotContainEntity);

            Assert.Equal(1, computedGroup.CachedEntities.Count);
            Assert.Equal(1, firedTimes);
            Assert.Contains(shouldContainEntity, computedGroup.CachedEntities);
            Assert.DoesNotContain(shouldNotContainEntity, computedGroup.CachedEntities);
        }

        [Fact]
        public void should_only_remove_and_fire_events_when_non_applicable_entity_removed()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var shouldContainEntity = Substitute.For<IEntity>();
            shouldContainEntity.Id.Returns(1);
            shouldContainEntity.HasComponent<TestComponentOne>().Returns(true);

            var dummyEntitySnapshot = new List<IEntity> {shouldContainEntity};
            mockObservableGroup.GetEnumerator().Returns(x => dummyEntitySnapshot.GetEnumerator());

            var onEntityRemovingSubject = new Subject<IEntity>();
            var onEntityRemovedSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(onEntityRemovingSubject);
            mockObservableGroup.OnEntityRemoved.Returns(onEntityRemovedSubject);

            var computedGroup = new TestComputedGroup(mockObservableGroup);

            var removingFiredTimes = 0;
            computedGroup.OnEntityRemoving.Subscribe(x =>
            {
                Assert.Equal(shouldContainEntity, x);
                removingFiredTimes++;
            });

            var removedFiredTimes = 0;
            computedGroup.OnEntityRemoved.Subscribe(x =>
            {
                Assert.Equal(shouldContainEntity, x);
                removedFiredTimes++;
            });

            shouldContainEntity.HasComponent<TestComponentOne>().Returns(false);
            computedGroup.RefreshEntities();

            Assert.Equal(0, computedGroup.CachedEntities.Count);
            Assert.Equal(1, removingFiredTimes);
            Assert.Equal(1, removedFiredTimes);
        }

        [Fact]
        public void should_only_fire_events_from_refresh_when_cache_changed()
        {

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent<TestComponentOne>().Returns(false);

            var inapplicableEntity = Substitute.For<IEntity>();
            inapplicableEntity.Id.Returns(2);
            inapplicableEntity.HasComponent<TestComponentOne>().Returns(false);

            var dummyEntitySnapshot = new List<IEntity> { applicableEntity, inapplicableEntity };

            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.GetEnumerator().Returns(x => dummyEntitySnapshot.GetEnumerator());
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(Observable.Empty<IEntity>());

            var computedGroup = new TestComputedGroup(mockObservableGroup);

            var addedFiredTimes = 0;
            computedGroup.OnEntityAdded.Subscribe(x =>
            {
                addedFiredTimes++;
            });

            var removingFiredTimes = 0;
            computedGroup.OnEntityRemoving.Subscribe(x =>
            {
                removingFiredTimes++;
            });

            var removedFiredTimes = 0;
            computedGroup.OnEntityRemoved.Subscribe(x =>
            {
                removedFiredTimes++;
            });

            //No events should fire
            computedGroup.RefreshEntities();

            Assert.Equal(0, addedFiredTimes);
            Assert.Equal(0, removingFiredTimes);
            Assert.Equal(0, removedFiredTimes);

            applicableEntity.HasComponent<TestComponentOne>().Returns(true);

            //Add should fire only once
            computedGroup.RefreshEntities();

            Assert.Single(computedGroup.CachedEntities);
            Assert.Equal(1, addedFiredTimes);
            Assert.Equal(0, removingFiredTimes);
            Assert.Equal(0, removedFiredTimes);

            applicableEntity.HasComponent<TestComponentOne>().Returns(false);

            //Remove should fire only once
            computedGroup.RefreshEntities();

            Assert.Empty(computedGroup.CachedEntities);
            Assert.Equal(1, addedFiredTimes);
            Assert.Equal(1, removingFiredTimes);
            Assert.Equal(1, removedFiredTimes);
        }
    }
}