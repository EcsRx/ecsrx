using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups.Observable;
using EcsRx.Reactive;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ObservableGroupTests
    {
        [Fact]
        public void should_include_entity_snapshot_on_creation()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new Type[] { }, "default");

            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                new Entity(Guid.NewGuid(), mockEventSystem),
                new Entity(Guid.NewGuid(), mockEventSystem),
                new Entity(Guid.NewGuid(), mockEventSystem)
            };

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, dummyEntitySnapshot);

            Assert.Equal(3, cacheableGroupAccessor.CachedEntities.Count);
            Assert.Equal(dummyEntitySnapshot[0], cacheableGroupAccessor.CachedEntities[dummyEntitySnapshot[0].Id]);
            Assert.Equal(dummyEntitySnapshot[1], cacheableGroupAccessor.CachedEntities[dummyEntitySnapshot[1].Id]);
            Assert.Equal(dummyEntitySnapshot[2], cacheableGroupAccessor.CachedEntities[dummyEntitySnapshot[2].Id]);

            cacheableGroupAccessor.Dispose();
        }

        [Fact]
        public void should_only_cache_applicable_entity_when_applicable_entity_added()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var collectionName = "defaut";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, collectionName);
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);
            
            var applicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            applicableEntity.AddComponent<TestComponentOne>();
            applicableEntity.AddComponent<TestComponentTwo>();

            var unapplicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            unapplicableEntity.AddComponent<TestComponentOne>();

            var underlyingEvent = new ReactiveProperty<EntityAddedEvent>(new EntityAddedEvent(applicableEntity, mockCollection));
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { });
            underlyingEvent.SetValueAndForceNotify(new EntityAddedEvent(unapplicableEntity, mockCollection));
            
            Assert.Equal(1, cacheableGroupAccessor.CachedEntities.Count);
            Assert.Equal<IEntity>(applicableEntity, cacheableGroupAccessor.CachedEntities[applicableEntity.Id]);
        }

        [Fact]
        public void should_not_cache_applicable_entity_when_added_to_different_collection()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var collectionName = "defaut";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, "some-other-entityCollection-name");
            var mockCollection = Substitute.For<IEntityCollection>();
            mockCollection.Name.Returns(collectionName);

            var applicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            applicableEntity.AddComponent<TestComponentOne>();
            applicableEntity.AddComponent<TestComponentTwo>();

            var unapplicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            unapplicableEntity.AddComponent<TestComponentOne>();

            var underlyingEvent = new ReactiveProperty<EntityAddedEvent>(new EntityAddedEvent(applicableEntity, mockCollection));
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { });
            underlyingEvent.SetValueAndForceNotify(new EntityAddedEvent(unapplicableEntity, mockCollection));

            Assert.Empty(cacheableGroupAccessor.CachedEntities);
        }

        [Fact]
        public void should_only_remove_applicable_entity_when_entity_removed()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, "default");
            var mockCollection = Substitute.For<IEntityCollection>();

            var existingEntityOne = new Entity(Guid.NewGuid(), mockEventSystem);
            existingEntityOne.AddComponent<TestComponentOne>();
            existingEntityOne.AddComponent<TestComponentTwo>();

            var existingEntityTwo = new Entity(Guid.NewGuid(), mockEventSystem);
            existingEntityTwo.AddComponent<TestComponentOne>();
            existingEntityTwo.AddComponent<TestComponentTwo>();

            var unapplicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            unapplicableEntity.AddComponent<TestComponentOne>();

            var underlyingEvent = new ReactiveProperty<EntityRemovedEvent>(new EntityRemovedEvent(unapplicableEntity, mockCollection));
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { existingEntityOne, existingEntityTwo });
            underlyingEvent.SetValueAndForceNotify(new EntityRemovedEvent(existingEntityOne, mockCollection));

            Assert.Equal(1, cacheableGroupAccessor.CachedEntities.Count);
            Assert.Equal<IEntity>(existingEntityTwo, cacheableGroupAccessor.CachedEntities[existingEntityTwo.Id]);
        }

        [Fact]
        public void should_only_remove_entity_when_components_no_longer_match_group()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, "default");

            var existingEntityOne = new Entity(Guid.NewGuid(), mockEventSystem);
            var componentToRemove = new TestComponentOne();
            existingEntityOne.AddComponent(componentToRemove);
            existingEntityOne.AddComponent<TestComponentTwo>();

            var existingEntityTwo = new Entity(Guid.NewGuid(), mockEventSystem);
            var unapplicableComponent = new TestComponentThree();
            existingEntityTwo.AddComponent<TestComponentOne>();
            existingEntityTwo.AddComponent<TestComponentTwo>();
            existingEntityTwo.AddComponent(unapplicableComponent);

            var dummyEventToSeedMock = new ComponentsRemovedEvent(new Entity(Guid.NewGuid(), mockEventSystem), new[] {new TestComponentOne()});
            var underlyingEvent = new ReactiveProperty<ComponentsRemovedEvent>(dummyEventToSeedMock);
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { existingEntityOne, existingEntityTwo });
            existingEntityOne.RemoveComponent(componentToRemove);
            underlyingEvent.SetValueAndForceNotify(new ComponentsRemovedEvent(existingEntityOne, new [] {componentToRemove}));

            existingEntityTwo.RemoveComponent(unapplicableComponent);
            underlyingEvent.SetValueAndForceNotify(new ComponentsRemovedEvent(existingEntityTwo, new[] {unapplicableComponent}));

            Assert.Equal(1, cacheableGroupAccessor.CachedEntities.Count);
            Assert.Equal<IEntity>(existingEntityTwo, cacheableGroupAccessor.CachedEntities[existingEntityTwo.Id]);
        }

        [Fact]
        public void should_only_add_entity_when_components_match_group()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, "default");

            var existingEntityOne = new Entity(Guid.NewGuid(), mockEventSystem);
            var componentToAdd = new TestComponentOne();
            existingEntityOne.AddComponent<TestComponentTwo>();

            var existingEntityTwo = new Entity(Guid.NewGuid(), mockEventSystem);
            var unapplicableComponent = new TestComponentThree();
            existingEntityTwo.AddComponent<TestComponentOne>();

            var dummyEventToSeedMock = new ComponentsAddedEvent(new Entity(Guid.NewGuid(), mockEventSystem), new[]{new TestComponentOne()});
            var underlyingEvent = new ReactiveProperty<ComponentsAddedEvent>(dummyEventToSeedMock);
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] {});
            existingEntityOne.AddComponent(componentToAdd);
            underlyingEvent.SetValueAndForceNotify(new ComponentsAddedEvent(existingEntityOne, new[]{componentToAdd}));

            existingEntityTwo.AddComponent(unapplicableComponent);
            underlyingEvent.SetValueAndForceNotify(new ComponentsAddedEvent(existingEntityTwo, new[]{unapplicableComponent}));

            Assert.Equal(1, cacheableGroupAccessor.CachedEntities.Count);
            Assert.Equal<IEntity>(existingEntityOne, cacheableGroupAccessor.CachedEntities[existingEntityOne.Id]);
        }
        
        [Fact]
        public void should_correctly_notify_when_entity_added()
        {
            var componentTypes = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockCollection = Substitute.For<IEntityCollection>();
            var accessorToken = new ObservableGroupToken(componentTypes, "default");
            
            var fakeEntity1 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();

            var fakeEntity2 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            mockCollection.Name.Returns("default");
            
            var underlyingEvent = new Subject<EntityAddedEvent>();
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            
            var observableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[0]);
            ObservableExtensions.Subscribe<IEntity>(observableGroup.OnEntityAdded, x =>
            {
                Assert.Equal<IEntity>(fakeEntity1, x);
                timesCalled++;
            });
            
            underlyingEvent.OnNext(new EntityAddedEvent(fakeEntity1, mockCollection));
            underlyingEvent.OnNext(new EntityAddedEvent(fakeEntity2, mockCollection));

            Assert.Equal(1, timesCalled);
        }

        [Fact]
        public void should_correctly_notify_when_matching_entity_removed()
        {
            var componentTypes = new[] { typeof(TestComponentOne), typeof(TestComponentTwo) };
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockCollection = Substitute.For<IEntityCollection>();
            var accessorToken = new ObservableGroupToken(componentTypes, "default");

            var fakeEntity1 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();

            var fakeEntity2 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            mockCollection.Name.Returns("default");
            
            var underlyingEvent = new Subject<EntityRemovedEvent>();
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(underlyingEvent);
            
            var observableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[]{ fakeEntity1 });
            ObservableExtensions.Subscribe<IEntity>(observableGroup.OnEntityRemoved, x =>
            {
                Assert.Equal<IEntity>(fakeEntity1, x);
                timesCalled++;
            });

            underlyingEvent.OnNext(new EntityRemovedEvent(fakeEntity1, mockCollection));
            underlyingEvent.OnNext(new EntityRemovedEvent(fakeEntity2, mockCollection));

            Assert.Equal(1, timesCalled);
        }

        [Fact]
        public void should_correctly_notify_when_matching_component_added()
        {
            var componentTypes = new[] { typeof(TestComponentOne), typeof(TestComponentTwo) };
            var fakeEventSystem = new EventSystem(new MessageBroker());
            var accessorToken = new ObservableGroupToken(componentTypes, "default");

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();

            var timesCalled = 0;
            
            var observableGroup = new ObservableGroup(fakeEventSystem, accessorToken, new IEntity[0]);
            ObservableExtensions.Subscribe<IEntity>(observableGroup.OnEntityAdded, x =>
            {
                Assert.Equal<IEntity>(fakeEntity1, x);
                timesCalled++;
            });
            
            fakeEntity1.AddComponent<TestComponentTwo>();
            fakeEntity1.AddComponent<TestComponentThree>();
            fakeEntity2.AddComponent<TestComponentThree>();

            Assert.Equal(1, timesCalled);
        }

        [Fact]
        public void should_correctly_notify_when_matching_component_removed()
        {
            var componentTypes = new[] { typeof(TestComponentOne), typeof(TestComponentTwo) };
            var fakeEventSystem = new EventSystem(new MessageBroker());
            var accessorToken = new ObservableGroupToken(componentTypes, "default");

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();
            fakeEntity1.AddComponent<TestComponentThree>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            
            var observableGroup = new ObservableGroup(fakeEventSystem, accessorToken, new IEntity[]{fakeEntity1});
            ObservableExtensions.Subscribe<IEntity>(observableGroup.OnEntityRemoved, x =>
            {
                Assert.Equal<IEntity>(fakeEntity1, x);
                timesCalled++;
            });
            
            fakeEntity1.RemoveComponent<TestComponentThree>();
            fakeEntity1.RemoveComponent<TestComponentTwo>();
            fakeEntity2.RemoveComponent<TestComponentThree>();

            Assert.Equal(1, timesCalled);
        }
    }
}