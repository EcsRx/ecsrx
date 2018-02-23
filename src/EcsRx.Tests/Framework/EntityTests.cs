using System;
using System.Linq;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests
{
    public class EntityTests
    {
        [Fact]
        public void should_correctly_add_component()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = Substitute.For<IComponent>();

            entity.AddComponent(dummyComponent);

            Assert.Equal(1, entity.Components.Count());
        }

        [Fact]
        public void should_correctly_raise_event_when_adding_component()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = Substitute.For<IComponent>();

            entity.AddComponent(dummyComponent);

            mockEventSystem.Received().Publish(Arg.Is<ComponentsAddedEvent>(x => x.Entity == entity && x.Components.Contains(dummyComponent)));
        }

        [Fact]
        public void should_correctly_remove_component()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = Substitute.For<IComponent>();
            entity.AddComponent(dummyComponent);
            
            entity.RemoveComponent(dummyComponent);

            Assert.Empty(entity.Components);
        }

        [Fact]
        public void should_correctly_raise_event_when_removing_component()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = Substitute.For<IComponent>();
            entity.AddComponent(dummyComponent);
            entity.RemoveComponent(dummyComponent);

            mockEventSystem.Received().Publish(Arg.Is<ComponentsRemovedEvent>(x => x.Entity == entity && x.Components[0] == dummyComponent));
        }

        [Fact]
        public void should_return_true_when_entity_has_component()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = new TestComponentOne();
            entity.AddComponent(dummyComponent);

            Assert.True(entity.HasComponent<TestComponentOne>());
        }

        [Fact]
        public void should_return_true_when_entity_matches_all_components()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            entity.AddComponent(new TestComponentOne());
            entity.AddComponent(new TestComponentTwo());

            Assert.True(entity.HasComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_all_components()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            entity.AddComponent(new TestComponentOne());

            Assert.False(entity.HasComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_is_missing_component()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);

            Assert.False(entity.HasComponent<TestComponentOne>());
        }

        [Fact]
        public void should_throw_error_when_adding_component_that_already_exists()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = Substitute.For<IComponent>();
            entity.AddComponent(dummyComponent);

            Assert.Throws<ArgumentException>(() => entity.AddComponent(dummyComponent));
        }

        [Fact]
        public void should_not_throw_error_when_removing_non_existent_component_with_generic()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            entity.RemoveComponent<TestComponentOne>();
        }

        [Fact]
        public void should_not_throw_error_when_removing_non_existent_component_with_instance()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var dummyComponent = new TestComponentOne();

            entity.RemoveComponent(dummyComponent);
        }

        [Fact]
        public void should_remove_all_components_when_disposing()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();

            var entity = new Entity(Guid.NewGuid(), mockEventSystem);
            var testComponentOne = entity.AddComponent<TestComponentOne>();
            var testComponentTwo = entity.AddComponent<TestComponentTwo>();
            var testComponentThree = entity.AddComponent<TestComponentThree>();

            entity.Dispose();

            mockEventSystem.Received().Publish(Arg.Is<ComponentsRemovedEvent>(x => x.Entity == entity && x.Components.Length == 3));

            Assert.Empty(entity.Components);
        }
    }
}
