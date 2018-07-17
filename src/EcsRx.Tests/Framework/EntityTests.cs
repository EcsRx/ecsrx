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
        public void should_correctly_raise_event_when_adding_component()
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
        public void should_correctly_raise_event_when_removing_component()
        {
            var componentRepository = Substitute.For<IComponentRepository>();
            var entity = new Entity(1, componentRepository);
            var dummyComponent = Substitute.For<IComponent>();

            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x => beforeWasCalled = true);
            entity.ComponentsRemoved.Subscribe(x => afterWasCalled = true);

            entity.RemoveComponent(dummyComponent);
            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
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
                
            var entity = new Entity(fakeEntityId, componentRepository);

            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x =>
            {
                beforeWasCalled = true;
                Assert.All(x, y => fakeComponents.Contains(y));
            });
            entity.ComponentsRemoved.Subscribe(x =>
            {
                afterWasCalled = true;
                Assert.All(x, y => fakeComponents.Contains(y));
            });
            
            entity.Dispose();
            componentRepository.GetAll(fakeEntityId).Returns(new IComponent[0]);

            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
            Assert.Empty(entity.Components);
        }
    }
}
