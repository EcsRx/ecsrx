using System;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class EntityTests
    {
        [Fact]
        public void should_raise_event_when_adding_component()
        {
            var componentRepository = Substitute.For<IComponentRepository>();
            var entity = new Entity(1, componentRepository);
            var dummyComponent = Substitute.For<IComponent>();

            var wasCalled = false;
            entity.ComponentsAdded.Subscribe(x => wasCalled = true);

            entity.AddComponent(dummyComponent);
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_raise_event_when_removing_component_that_exists()
        {
            var componentRepository = Substitute.For<IComponentRepository>();
            var entity = new Entity(1, componentRepository);
            var dummyComponent = Substitute.For<IComponent>();

            componentRepository.Has(Arg.Any<int>(), dummyComponent.GetType()).Returns(true);

            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x => beforeWasCalled = true);
            entity.ComponentsRemoved.Subscribe(x => afterWasCalled = true);

            entity.RemoveComponent(dummyComponent);
            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
        }
        
        [Fact]
        public void should_not_raise_events_or_throw_when_removing_non_existent_components()
        {
            var componentRepository = Substitute.For<IComponentRepository>();
            var entity = new Entity(1, componentRepository);

            componentRepository.Has(Arg.Any<int>(), Arg.Any<Type>()).Returns(false);
            
            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x => beforeWasCalled = true);
            entity.ComponentsRemoved.Subscribe(x => afterWasCalled = true);

            entity.RemoveComponent<TestComponentOne>();
            Assert.False(beforeWasCalled);
            Assert.False(afterWasCalled);
        }

        [Fact]
        public void should_return_true_when_entity_has_all_components()
        {
            var fakeEntityId = 1;
            var componentRepository = Substitute.For<IComponentRepository>();
            componentRepository.Has(fakeEntityId, typeof(TestComponentOne)).Returns(true);
            componentRepository.Has(fakeEntityId, typeof(TestComponentTwo)).Returns(true);
            var entity = new Entity(fakeEntityId, componentRepository);
            
            Assert.True(entity.HasAllComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_all_components()
        {
            var fakeEntityId = 1;
            var componentRepository = Substitute.For<IComponentRepository>();
            componentRepository.Has(fakeEntityId, typeof(TestComponentOne)).Returns(true);
            componentRepository.Has(fakeEntityId, typeof(TestComponentTwo)).Returns(false);
            var entity = new Entity(fakeEntityId, componentRepository);
            
            Assert.False(entity.HasAllComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }
        
        [Fact]
        public void should_return_true_when_entity_has_any_components()
        {
            var fakeEntityId = 1;
            var componentRepository = Substitute.For<IComponentRepository>();
            componentRepository.Has(fakeEntityId, typeof(TestComponentOne)).Returns(true);
            componentRepository.Has(fakeEntityId, typeof(TestComponentTwo)).Returns(false);
            var entity = new Entity(fakeEntityId, componentRepository);
            
            Assert.True(entity.HasAnyComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_any_components()
        {
            var fakeEntityId = 1;
            var componentRepository = Substitute.For<IComponentRepository>();
            componentRepository.Has(fakeEntityId, typeof(TestComponentOne)).Returns(false);
            componentRepository.Has(fakeEntityId, typeof(TestComponentTwo)).Returns(false);
            var entity = new Entity(fakeEntityId, componentRepository);
            
            Assert.False(entity.HasAnyComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_remove_all_components_when_disposing()
        {
            var fakeEntityId = 1;
            var fakeComponents = new IComponent[] {new TestComponentOne(), new TestComponentTwo()};

            var componentRepository = Substitute.For<IComponentRepository>();
            componentRepository.GetAll(fakeEntityId).Returns(fakeComponents);

            componentRepository.Has(Arg.Any<int>(), Arg.Any<Type>()).Returns(true);

            var entity = new Entity(fakeEntityId, componentRepository);

            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x =>
            {
                beforeWasCalled = true;
                var fakeComponentTypes = fakeComponents.Select(y => y.GetType());
                Assert.All(x, y => fakeComponentTypes.Contains(y));
            });
            entity.ComponentsRemoved.Subscribe(x =>
            {
                afterWasCalled = true;
                var fakeComponentTypes = fakeComponents.Select(y => y.GetType());
                Assert.All(x, y => fakeComponentTypes.Contains(y));
            });
            
            entity.Dispose();
            componentRepository.GetAll(fakeEntityId).Returns(new IComponent[0]);

            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
            Assert.Empty(entity.Components);
        }
    }
}
