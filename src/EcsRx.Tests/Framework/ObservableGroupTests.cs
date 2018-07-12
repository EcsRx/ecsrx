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
            var accessorToken = new ObservableGroupToken(new Type[0], new Type[0], "default");

            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            
            var dummyEntitySnapshot = new List<IEntity>
            {
                new Entity(Guid.NewGuid(), mockEventSystem),
                new Entity(Guid.NewGuid(), mockEventSystem),
                new Entity(Guid.NewGuid(), mockEventSystem)
            };

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, dummyEntitySnapshot);

            Assert.Equal(3, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(dummyEntitySnapshot[0], cacheableobservableGroup.CachedEntities[dummyEntitySnapshot[0].Id]);
            Assert.Equal(dummyEntitySnapshot[1], cacheableobservableGroup.CachedEntities[dummyEntitySnapshot[1].Id]);
            Assert.Equal(dummyEntitySnapshot[2], cacheableobservableGroup.CachedEntities[dummyEntitySnapshot[2].Id]);

            cacheableobservableGroup.Dispose();
        }

        [Fact]
        public void should_only_cache_applicable_entity_when_applicable_entity_added()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], collectionName);
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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { });
            underlyingEvent.SetValueAndForceNotify(new EntityAddedEvent(unapplicableEntity, mockCollection));
            
            Assert.Equal(1, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(applicableEntity, cacheableobservableGroup.CachedEntities[applicableEntity.Id]);
        }

        [Fact]
        public void should_not_cache_applicable_entity_when_added_to_different_collection()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var collectionName = "default";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], "some-other-entityCollection-name");
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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { }, mockCollection);
            underlyingEvent.SetValueAndForceNotify(new EntityAddedEvent(unapplicableEntity, mockCollection));

            Assert.Empty(cacheableobservableGroup.CachedEntities);
        }

        [Fact]
        public void should_only_remove_applicable_entity_when_entity_removed()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], "default");
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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { existingEntityOne, existingEntityTwo });
            underlyingEvent.SetValueAndForceNotify(new EntityRemovedEvent(existingEntityOne, mockCollection));

            Assert.Equal(1, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(existingEntityTwo, cacheableobservableGroup.CachedEntities[existingEntityTwo.Id]);
        }

        [Fact]
        public void should_remove_entity_when_required_components_no_longer_match_group()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], "default");

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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { existingEntityOne, existingEntityTwo });
            existingEntityOne.RemoveComponent(componentToRemove);
            underlyingEvent.SetValueAndForceNotify(new ComponentsRemovedEvent(existingEntityOne, new [] {componentToRemove}));

            existingEntityTwo.RemoveComponent(unapplicableComponent);
            underlyingEvent.SetValueAndForceNotify(new ComponentsRemovedEvent(existingEntityTwo, new[] {unapplicableComponent}));

            Assert.Equal(1, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(existingEntityTwo, cacheableobservableGroup.CachedEntities[existingEntityTwo.Id]);
        }
        
        [Fact]
        public void should_add_entity_when_required_components_match_and_excluded_component_removed()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new [] { typeof(TestComponentThree)}, "default");

            var entityToAdd = new Entity(Guid.NewGuid(), mockEventSystem);
            var componentToRemove = new TestComponentThree();
            entityToAdd.AddComponent<TestComponentOne>();
            entityToAdd.AddComponent<TestComponentTwo>();
            entityToAdd.AddComponent(componentToRemove);

            var underlyingEvent = new Subject<ComponentsRemovedEvent>();
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[0]);
            entityToAdd.RemoveComponent(componentToRemove);
            underlyingEvent.OnNext(new ComponentsRemovedEvent(entityToAdd, new[]{componentToRemove}));

            Assert.Equal(1, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(entityToAdd, cacheableobservableGroup.CachedEntities[entityToAdd.Id]);
        }

        [Fact]
        public void should_only_add_entity_when_components_match_group()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, new Type[0], "default");

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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());

            var cacheableobservableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] {});
            existingEntityOne.AddComponent(componentToAdd);
            underlyingEvent.SetValueAndForceNotify(new ComponentsAddedEvent(existingEntityOne, new[]{componentToAdd}));

            existingEntityTwo.AddComponent(unapplicableComponent);
            underlyingEvent.SetValueAndForceNotify(new ComponentsAddedEvent(existingEntityTwo, new[]{unapplicableComponent}));

            Assert.Equal(1, cacheableobservableGroup.CachedEntities.Count);
            Assert.Equal(existingEntityOne, cacheableobservableGroup.CachedEntities[existingEntityOne.Id]);
        }
        
        [Fact]
        public void should_correctly_notify_when_entity_added()
        {
            var componentTypes = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockCollection = Substitute.For<IEntityCollection>();
            var accessorToken = new ObservableGroupToken(componentTypes, new Type[0], "default");
            
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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            
            var observableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[0]);
            observableGroup.OnEntityAdded.Subscribe(x =>
            {
                Assert.Equal(fakeEntity1, x);
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
            var accessorToken = new ObservableGroupToken(componentTypes, new Type[0], "default");

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
            mockEventSystem.Receive<ComponentsBeforeRemovedEvent>().Returns(Observable.Empty<ComponentsBeforeRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(underlyingEvent);
            
            var observableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[]{ fakeEntity1 });
            observableGroup.OnEntityRemoved.Subscribe(x =>
            {
                Assert.Equal(fakeEntity1, x);
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
            var accessorToken = new ObservableGroupToken(componentTypes, new Type[0], "default");

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();

            var timesCalled = 0;
            
            var observableGroup = new ObservableGroup(fakeEventSystem, accessorToken, new IEntity[0]);
            observableGroup.OnEntityAdded.Subscribe(x =>
            {
                Assert.Equal(fakeEntity1, x);
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
            var accessorToken = new ObservableGroupToken(componentTypes, new Type[0], "default");

            var fakeEntity1 = new Entity(Guid.NewGuid(), fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();
            fakeEntity1.AddComponent<TestComponentThree>();

            var fakeEntity2 = new Entity(Guid.NewGuid(), fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            
            var observableGroup = new ObservableGroup(fakeEventSystem, accessorToken, new IEntity[]{fakeEntity1});
            observableGroup.OnEntityRemoved.Subscribe(x =>
            {
                Assert.Equal(fakeEntity1, x);
                timesCalled++;
            });
            
            fakeEntity1.RemoveComponent<TestComponentThree>();
            fakeEntity1.RemoveComponent<TestComponentTwo>();
            fakeEntity2.RemoveComponent<TestComponentThree>();

            Assert.Equal(1, timesCalled);
        }
    }
}