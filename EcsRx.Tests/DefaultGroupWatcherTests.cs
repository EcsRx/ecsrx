using System;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups.Watchers;
using EcsRx.Reactive;
using EcsRx.Tests.Components;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class DefaultGroupWatcherTests
    {
        [Test]
        public void should_correctly_notify_when_matching_entity_added()
        {
            var componentTypes = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var fakeEventSystem = new EventSystem(new MessageBroker());

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;

            var groupWatcher = new DefaultGroupWatcher(fakeEventSystem, componentTypes);
            groupWatcher.OnEntityAdded.Subscribe(x =>
            {
                Assert.That(x, Is.EqualTo(fakeEntity1));
                timesCalled++;
            });

            fakeEventSystem.Publish(new EntityAddedEvent(fakeEntity1, null));
            fakeEventSystem.Publish(new EntityAddedEvent(fakeEntity2, null));

            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
        public void should_correctly_notify_when_matching_entity_removed()
        {
            var componentTypes = new[] { typeof(TestComponentOne), typeof(TestComponentTwo) };
            var fakeEventSystem = new EventSystem(new MessageBroker());

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;

            var groupWatcher = new DefaultGroupWatcher(fakeEventSystem, componentTypes);
            groupWatcher.OnEntityRemoved.Subscribe(x =>
            {
                Assert.That(x, Is.EqualTo(fakeEntity1));
                timesCalled++;
            });

            fakeEventSystem.Publish(new EntityRemovedEvent(fakeEntity1, null));
            fakeEventSystem.Publish(new EntityRemovedEvent(fakeEntity2, null));

            Assert.That(timesCalled, Is.EqualTo(1));
        }

        [Test]
        public void should_correctly_notify_when_matching_component_added()
        {
            var componentTypes = new[] { typeof(TestComponentOne), typeof(TestComponentTwo) };
            var fakeEventSystem = new EventSystem(new MessageBroker());

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();

            var timesCalled = 0;
            var groupWatcher = new DefaultGroupWatcher(fakeEventSystem, componentTypes);
            groupWatcher.OnEntityAdded.Subscribe(x =>
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

            var fakeEntity1 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity1.AddComponent<TestComponentOne>();
            fakeEntity1.AddComponent<TestComponentTwo>();
            fakeEntity1.AddComponent<TestComponentThree>();

            var fakeEntity2 = new Entity(Guid.Empty, fakeEventSystem);
            fakeEntity2.AddComponent<TestComponentOne>();
            fakeEntity2.AddComponent<TestComponentThree>();

            var timesCalled = 0;
            var groupWatcher = new DefaultGroupWatcher(fakeEventSystem, componentTypes);
            groupWatcher.OnEntityRemoved.Subscribe(x =>
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