using System;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
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

            entity.AddComponents(dummyComponent);
            Assert.True(wasCalled);
        }

        [Fact]
        public void should_raise_event_when_removing_component_that_exists()
        {
            var componentRepository = Substitute.For<IComponentRepository>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entity = new Entity(1, componentRepository);
            var dummyComponent = Substitute.For<IComponent>();

            componentRepository.ComponentTypeLookup.Returns(componentTypeLookup);
            componentRepository.Has(Arg.Any<int>(), 1).Returns(true);
            componentTypeLookup.GetComponentTypes(Arg.Any<Type>()).Returns(new []{1});

            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x => beforeWasCalled = true);
            entity.ComponentsRemoved.Subscribe(x => afterWasCalled = true);

            entity.RemoveComponents(dummyComponent.GetType());
            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
        }
        
        [Fact]
        public void should_not_raise_events_or_throw_when_removing_non_existent_components()
        {
            var componentRepository = Substitute.For<IComponentRepository>();
            var entity = new Entity(1, componentRepository);

            componentRepository.Has(Arg.Any<int>(), Arg.Any<int>()).Returns(false);
            
            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.ComponentsRemoving.Subscribe(x => beforeWasCalled = true);
            entity.ComponentsRemoved.Subscribe(x => afterWasCalled = true);

            entity.RemoveComponents(typeof(TestComponentOne));
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
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentType(typeof(TestComponentTwo)).Returns(1);
            
            var componentRepository = Substitute.For<IComponentRepository>();
            componentRepository.Has(fakeEntityId, 0).Returns(true);
            componentRepository.Has(fakeEntityId, 1).Returns(false);
            componentRepository.ComponentTypeLookup.Returns(componentTypeLookup);

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

            var componentRepository = Substitute.For<IComponentRepository>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            
            componentTypeLookup.GetComponentTypes(Arg.Any<Type[]>()).Returns(new []{1,2});
            componentRepository.Has(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            var expectedRange = Enumerable.Range(1, 2);
            var entity = new Entity(fakeEntityId, componentRepository);
            entity.ActiveComponents.AddRange(expectedRange);
            
            var beforeWasCalled = false;
            var afterWasCalled = false;
            
            entity.ComponentsRemoving.Subscribe(x =>
            {
                beforeWasCalled = true;
                Assert.All(x, y => expectedRange.Contains(y));
            });
            entity.ComponentsRemoved.Subscribe(x =>
            {
                afterWasCalled = true;
                Assert.All(x, y => expectedRange.Contains(y));
            });
            
            entity.Dispose();

            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
            Assert.Empty(entity.Components);
            Assert.Empty(entity.ActiveComponents);
        }

        [Fact]
        public void should_add_components_in_parameter_order()
        {
            var fakeEntityId = 1;
            var fakeComponents = new IComponent[] {new TestComponentOne(), new TestComponentTwo(), new TestComponentThree()};

            var componentRepository = Substitute.For<IComponentRepository>();
            var entity = new Entity(fakeEntityId, componentRepository);
            entity.AddComponents(fakeComponents);
            
            Received.InOrder(() => {
                componentRepository.Add(fakeEntityId, fakeComponents[0]);
                componentRepository.Add(fakeEntityId, fakeComponents[1]);
                componentRepository.Add(fakeEntityId, fakeComponents[2]);
            });
        }
    }
}
