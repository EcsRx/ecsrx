using System;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Pools;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using EcsRx.Groups.Accessors;
using EcsRx.Reactive;
using EcsRx.Tests.Components;
using NSubstitute.Extensions;
using NSubstitute.ReturnsExtensions;

namespace EcsRx.Tests
{
    [TestFixture]
    public class CacheableGroupAccessorTests
    {
        [Test]
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

            Assert.That(cacheableGroupAccessor.CachedEntities, Has.Count.EqualTo(3));
            Assert.That(cacheableGroupAccessor.CachedEntities[dummyEntitySnapshot[0].Id], Is.EqualTo(dummyEntitySnapshot[0]));
            Assert.That(cacheableGroupAccessor.CachedEntities[dummyEntitySnapshot[1].Id], Is.EqualTo(dummyEntitySnapshot[1]));
            Assert.That(cacheableGroupAccessor.CachedEntities[dummyEntitySnapshot[2].Id], Is.EqualTo(dummyEntitySnapshot[2]));

            cacheableGroupAccessor.Dispose();
        }

        [Test]
        public void should_only_cache_applicable_entity_when_applicable_entity_added()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var poolName = "defaut";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, poolName);
            var mockPool = Substitute.For<IPool>();
            mockPool.Name.Returns(poolName);
            
            var applicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            applicableEntity.AddComponent<TestComponentOne>();
            applicableEntity.AddComponent<TestComponentTwo>();

            var unapplicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            unapplicableEntity.AddComponent<TestComponentOne>();

            var underlyingEvent = new ReactiveProperty<EntityAddedEvent>(new EntityAddedEvent(applicableEntity, mockPool));
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { });
            underlyingEvent.SetValueAndForceNotify(new EntityAddedEvent(unapplicableEntity, mockPool));
            
            Assert.That(cacheableGroupAccessor.CachedEntities, Has.Count.EqualTo(1));
            Assert.That(cacheableGroupAccessor.CachedEntities[applicableEntity.Id], Is.EqualTo(applicableEntity));
        }

        [Test]
        public void should_not_cache_applicable_entity_when_added_to_different_pool()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var poolName = "defaut";
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, "some-other-pool-name");
            var mockPool = Substitute.For<IPool>();
            mockPool.Name.Returns(poolName);

            var applicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            applicableEntity.AddComponent<TestComponentOne>();
            applicableEntity.AddComponent<TestComponentTwo>();

            var unapplicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            unapplicableEntity.AddComponent<TestComponentOne>();

            var underlyingEvent = new ReactiveProperty<EntityAddedEvent>(new EntityAddedEvent(applicableEntity, mockPool));
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { });
            underlyingEvent.SetValueAndForceNotify(new EntityAddedEvent(unapplicableEntity, mockPool));

            Assert.That(cacheableGroupAccessor.CachedEntities, Is.Empty);
        }

        [Test]
        public void should_only_remove_applicable_entity_when_entity_removed()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var accessorToken = new ObservableGroupToken(new[] { typeof(TestComponentOne), typeof(TestComponentTwo) }, "default");
            var mockPool = Substitute.For<IPool>();

            var existingEntityOne = new Entity(Guid.NewGuid(), mockEventSystem);
            existingEntityOne.AddComponent<TestComponentOne>();
            existingEntityOne.AddComponent<TestComponentTwo>();

            var existingEntityTwo = new Entity(Guid.NewGuid(), mockEventSystem);
            existingEntityTwo.AddComponent<TestComponentOne>();
            existingEntityTwo.AddComponent<TestComponentTwo>();

            var unapplicableEntity = new Entity(Guid.NewGuid(), mockEventSystem);
            unapplicableEntity.AddComponent<TestComponentOne>();

            var underlyingEvent = new ReactiveProperty<EntityRemovedEvent>(new EntityRemovedEvent(unapplicableEntity, mockPool));
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());

            var cacheableGroupAccessor = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[] { existingEntityOne, existingEntityTwo });
            underlyingEvent.SetValueAndForceNotify(new EntityRemovedEvent(existingEntityOne, mockPool));

            Assert.That(cacheableGroupAccessor.CachedEntities, Has.Count.EqualTo(1));
            Assert.That(cacheableGroupAccessor.CachedEntities[existingEntityTwo.Id], Is.EqualTo(existingEntityTwo));
        }

        [Test]
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

            Assert.That(cacheableGroupAccessor.CachedEntities, Has.Count.EqualTo(1));
            Assert.That(cacheableGroupAccessor.CachedEntities[existingEntityTwo.Id], Is.EqualTo(existingEntityTwo));
        }

        [Test]
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

            Assert.That(cacheableGroupAccessor.CachedEntities, Has.Count.EqualTo(1));
            Assert.That(cacheableGroupAccessor.CachedEntities[existingEntityOne.Id], Is.EqualTo(existingEntityOne));
        }
        
        [Test]
        public void should_correctly_notify_when_entity_added()
        {
            var componentTypes = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockPool = Substitute.For<IPool>();
            var accessorToken = new ObservableGroupToken(componentTypes, "default");
            
            var fakeEntity1 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();

            var fakeEntity2 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            mockPool.Name.Returns("default");
            
            var underlyingEvent = new Subject<EntityAddedEvent>();
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(underlyingEvent);
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(Observable.Empty<EntityRemovedEvent>());
            
            var observableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[0]);
            observableGroup.OnEntityAdded.Subscribe(x =>
            {
                Assert.That(x, Is.EqualTo(fakeEntity1));
                timesCalled++;
            });
            
            underlyingEvent.OnNext(new EntityAddedEvent(fakeEntity1, mockPool));
            underlyingEvent.OnNext(new EntityAddedEvent(fakeEntity2, mockPool));

            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
        public void should_correctly_notify_when_matching_entity_removed()
        {
            var componentTypes = new[] { typeof(TestComponentOne), typeof(TestComponentTwo) };
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockPool = Substitute.For<IPool>();
            var accessorToken = new ObservableGroupToken(componentTypes, "default");

            var fakeEntity1 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();

            var fakeEntity2 = new Entity(Guid.Empty, mockEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            mockPool.Name.Returns("default");
            
            var underlyingEvent = new Subject<EntityRemovedEvent>();
            mockEventSystem.Receive<ComponentsAddedEvent>().Returns(Observable.Empty<ComponentsAddedEvent>());
            mockEventSystem.Receive<ComponentsRemovedEvent>().Returns(Observable.Empty<ComponentsRemovedEvent>());
            mockEventSystem.Receive<EntityAddedEvent>().Returns(Observable.Empty<EntityAddedEvent>());
            mockEventSystem.Receive<EntityRemovedEvent>().Returns(underlyingEvent);
            
            var observableGroup = new ObservableGroup(mockEventSystem, accessorToken, new IEntity[]{ fakeEntity1 });
            observableGroup.OnEntityRemoved.Subscribe(x =>
            {
                Assert.That(x, Is.EqualTo(fakeEntity1));
                timesCalled++;
            });

            underlyingEvent.OnNext(new EntityRemovedEvent(fakeEntity1, mockPool));
            underlyingEvent.OnNext(new EntityRemovedEvent(fakeEntity2, mockPool));

            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
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
            observableGroup.OnEntityAdded.Subscribe(x =>
            {
                Assert.That(x, Is.EqualTo(fakeEntity1));
                timesCalled++;
            });
            
            fakeEntity1.AddComponent<TestComponentTwo>();
            fakeEntity1.AddComponent<TestComponentThree>();
            fakeEntity2.AddComponent<TestComponentThree>();

            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
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
            observableGroup.OnEntityRemoved.Subscribe(x =>
            {
                Assert.That(x, Is.EqualTo(fakeEntity1));
                timesCalled++;
            });
            
            fakeEntity1.RemoveComponent<TestComponentThree>();
            fakeEntity1.RemoveComponent<TestComponentTwo>();
            fakeEntity2.RemoveComponent<TestComponentThree>();

            Assert.That(timesCalled, Is.EqualTo(1));
        }
    }
}