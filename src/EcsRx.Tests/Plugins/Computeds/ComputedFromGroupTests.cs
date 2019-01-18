using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Extensions;
using EcsRx.Tests.Models;
using EcsRx.Tests.Plugins.Computeds.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Plugins.Computeds
{
    public class ComputedFromGroupTests
    {
        [Fact]
        public void should_populate_on_creation()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new List<IEntity> {fakeEntity2, fakeEntity3}.Average(x => x.GetHashCode());

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);
            Assert.Equal(expectedData, computedGroupData.CachedData);            
        }
        
        [Fact]
        public void should_refresh_when_entities_added_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = fakeEntity3.GetHashCode();

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var addedEvent = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addedEvent);
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity2);
            addedEvent.OnNext(null);
            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);         
        }
        
        [Fact]
        public void should_refresh_when_entities_removed_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = fakeEntity3.GetHashCode();

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();

            var removedEvent = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(removedEvent);

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity2);
            removedEvent.OnNext(null);
            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);         
        }
        
        [Fact]
        public void should_refresh_on_trigger_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = fakeEntity3.GetHashCode();

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity2);
            computedGroupData.ManuallyRefresh.OnNext(true);
                       
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);         
        }
        
        [Fact]
        public void should_not_refresh_value_with_no_subs_and_value_requested()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new [] { fakeEntity2.GetHashCode(), fakeEntity3.GetHashCode() }.Average();

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(Observable.Empty<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(Observable.Empty<IEntity>());

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);

            fakeEntities.Remove(fakeEntity2);            
            var actualData = computedGroupData.Value;

            Assert.Equal(expectedData, actualData);         
        }
        
        [Fact]
        public void should_not_refresh_cached_data_on_change_notified_but_no_subs_without_value_request()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
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
            removingSubject.OnNext(null);
            computedGroupData.ManuallyRefresh.OnNext(true);

            Assert.Equal(expectedOutput, computedGroupData.CachedData);         
        }
        
        [Fact]
        public void should_refresh_cached_data_on_change_notified_with_active_subs_without_value_request()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.HasComponent<TestComponentThree>().Returns(false);
            
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity2.HasComponent<TestComponentThree>().Returns(true);

            var fakeEntity3 = Substitute.For<IEntity>();
            fakeEntity3.HasComponent<TestComponentThree>().Returns(true);

            var expectedData = new [] { fakeEntity2.GetHashCode(), fakeEntity3.GetHashCode() }.Average();

            var fakeEntities = new List<IEntity> {fakeEntity1, fakeEntity2, fakeEntity3};
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            
            var addingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addingSubject);

            var removingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityRemoving.Returns(removingSubject);

            mockObservableGroup.GetEnumerator().Returns(x => fakeEntities.GetEnumerator());
            
            var computedGroupData = new TestComputedFromGroup(mockObservableGroup);
            computedGroupData.Subscribe(x => {});

            fakeEntities.Remove(fakeEntity2);
            removingSubject.OnNext(null);
            Assert.NotEqual(expectedData, computedGroupData.CachedData);
            
            fakeEntities.Add(fakeEntity2);
            addingSubject.OnNext(null);  
            Assert.Equal(expectedData, computedGroupData.CachedData);

            fakeEntities.Remove(fakeEntity2);
            computedGroupData.ManuallyRefresh.OnNext(true);

            Assert.NotEqual(expectedData, computedGroupData.CachedData);         
        }
    }
}