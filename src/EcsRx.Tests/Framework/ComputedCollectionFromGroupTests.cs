using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Groups.Observable;
using EcsRx.Tests.ComputedGroups;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ComputedCollectionFromGroupTests
    {       
        [Fact]
        public void should_populate_on_creation()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new List<IEntity> {fakeEntity2, fakeEntity3}
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedCollectionFromGroup(mockObservableGroup);
            Assert.Equal(expectedData, computedGroupData.Value);            
        }
        
        [Fact]
        public void should_refresh_when_entities_added_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new List<IEntity> {fakeEntity2, fakeEntity3}
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var addedEvent = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addedEvent);
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedCollectionFromGroup(mockObservableGroup);

            fakeEntities.Add(fakeEntity3);
            addedEvent.OnNext(null);
            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);         
        }
        
        [Fact]
        public void should_refresh_when_entities_removed_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new[] {fakeEntity2.GetHashCode()};

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var removingEntity = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(removingEntity);

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedCollectionFromGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity3);
            removingEntity.OnNext(fakeEntity3);
            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);               
        }
        
        [Fact]
        public void should_refresh_on_trigger_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new List<IEntity> {fakeEntity2, fakeEntity3}
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var addedEvent = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addedEvent);
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedCollectionFromGroup(mockObservableGroup);

            fakeEntities.Add(fakeEntity3);
            computedGroupData.ManuallyRefresh.OnNext(true);
            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);              
        }
        
        [Fact]
        public void should_not_refresh_value_with_no_subs_events_or_triggers_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new List<IEntity> {fakeEntity2, fakeEntity3}
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedCollectionFromGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity2);            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);         
        }
        
        [Fact]
        public void should_not_refresh_cached_data_when_change_notified_but_no_value_request()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            
            var addingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addingSubject);

            var removingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityRemoving.Returns(removingSubject);

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);
            var expectedOutput = computedGroupData.CachedData;

            addingSubject.OnNext(null);
            computedGroupData.ManuallyRefresh.OnNext(true);

            Assert.Equal(expectedOutput, computedGroupData.CachedData);         
        }
        
        [Fact]
        public void should_refresh_cached_data_when_change_notified_with_subs_but_no_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(2);
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity2.Id.Returns(3);
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new List<IEntity> {fakeEntity2, fakeEntity3}
                .Select(x => x.GetHashCode())
                .OrderBy(x => x);

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            
            var addingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addingSubject);

            var removingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityRemoving.Returns(removingSubject);

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedCollectionFromGroup(mockObservableGroup);
            computedGroupData.Subscribe(x => {});

            fakeEntities.Remove(fakeEntity2);
            removingSubject.OnNext(fakeEntity2);
            Assert.False(expectedData.SequenceEqual(computedGroupData.Value));
            
            fakeEntities.Add(fakeEntity2);
            addingSubject.OnNext(fakeEntity2);  
            Assert.True(expectedData.SequenceEqual(computedGroupData.Value));

            fakeEntities.Remove(fakeEntity2);
            computedGroupData.ManuallyRefresh.OnNext(true);
            Assert.False(expectedData.SequenceEqual(computedGroupData.Value));      
        }
    }
}