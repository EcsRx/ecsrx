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
        public void should_raise_event_when_adding_components()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new int[1]);
            
            var entity = new Entity(1, componentDatabase, componentTypeLookup);
            var dummyComponent = Substitute.For<IComponent>();

            var wasCalled = false;
            entity.ComponentsAdded.Subscribe(x => wasCalled = true);

            entity.AddComponents(dummyComponent);
            Assert.True(wasCalled);
        }
        
        /* NSubstitute doesnt support ref returns currently
        [Fact]
        public void should_raise_event_when_adding_explicit_component()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            componentDatabase.GetRef<TestStructComponentOne>(Arg.Any<int>(), Arg.Any<int>()).Returns(new TestStructComponentOne());
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            
            var entity = new Entity(1, componentDatabase, componentTypeLookup);

            var wasCalled = false;
            entity.ComponentsAdded.Subscribe(x => wasCalled = true);

            entity.AddComponent<TestStructComponentOne>(0);
            Assert.True(wasCalled);
        }*/

        [Fact]
        public void should_raise_event_when_removing_component_that_exists()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new int[1]);
            componentTypeLookup.GetComponentTypes(Arg.Any<Type>()).Returns(new []{0});

            var entity = new Entity(1, componentDatabase, componentTypeLookup);
            var dummyComponent = Substitute.For<IComponent>();

            var beforeWasCalled = false;
            var afterWasCalled = false;
            entity.InternalComponentAllocations[0] = 1;
            entity.ComponentsRemoving.Subscribe(x => beforeWasCalled = true);
            entity.ComponentsRemoved.Subscribe(x => afterWasCalled = true);

            entity.RemoveComponents(dummyComponent.GetType());
            Assert.True(beforeWasCalled);
            Assert.True(afterWasCalled);
        }
        
        [Fact]
        public void should_not_raise_events_or_throw_when_removing_non_existent_components()
        {
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            var entity = new Entity(1, componentDatabase, componentTypeLookup);
            
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
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1});
            componentTypeLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentType(typeof(TestComponentTwo)).Returns(1);
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            entity.InternalComponentAllocations[0] = 1;
            entity.InternalComponentAllocations[1] = 1;
            Assert.True(entity.HasAllComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_all_components()
        {
            var fakeEntityId = 1;
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1});
            componentTypeLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentType(typeof(TestComponentTwo)).Returns(1);
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            entity.InternalComponentAllocations[0] = 1;
            Assert.False(entity.HasAllComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }
        
        [Fact]
        public void should_return_true_when_entity_has_any_components()
        {
            var fakeEntityId = 1;
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1});
            componentTypeLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            componentTypeLookup.GetComponentType(typeof(TestComponentTwo)).Returns(1);
            
            var componentDatabase = Substitute.For<IComponentDatabase>();
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            entity.InternalComponentAllocations[0] = 1;
            
            Assert.True(entity.HasAnyComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_return_false_when_entity_does_not_match_any_components()
        {
            var fakeEntityId = 1;
            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new int[1]);
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            
            Assert.False(entity.HasAnyComponents(typeof(TestComponentOne), typeof(TestComponentTwo)));
        }

        [Fact]
        public void should_remove_all_components_when_disposing()
        {
            var fakeEntityId = 1;

            var componentDatabase = Substitute.For<IComponentDatabase>();
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1, 2});
            
            var expectedRange = Enumerable.Range(0, 3);
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            entity.InternalComponentAllocations[0] = 1;
            entity.InternalComponentAllocations[1] = 1;
            entity.InternalComponentAllocations[2] = 1;
            
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
            Assert.All(entity.ComponentAllocations, i => i.Equals(Entity.NotAllocated));
        }

        [Fact]
        public void should_add_components_in_parameter_order()
        {
            var fakeEntityId = 1;
            var fakeComponents = new IComponent[] {new TestComponentOne(), new TestComponentTwo(), new TestComponentThree()};

            var componentDatabase = Substitute.For<IComponentDatabase>();
            componentDatabase.Allocate(Arg.Any<int>()).Returns(1);
            
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1, 2});
            componentTypeLookup.GetComponentType(fakeComponents[0].GetType()).Returns(0);
            componentTypeLookup.GetComponentType(fakeComponents[1].GetType()).Returns(1);
            componentTypeLookup.GetComponentType(fakeComponents[2].GetType()).Returns(2);
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            entity.AddComponents(fakeComponents);
            
            Received.InOrder(() => {
                componentDatabase.Allocate(0);
                componentDatabase.Set(0, 1, fakeComponents[0]);
                componentDatabase.Allocate(1);
                componentDatabase.Set(1, 1, fakeComponents[1]);
                componentDatabase.Allocate(2);
                componentDatabase.Set(2, 1, fakeComponents[2]);
            });
        }

        [Fact]
        public void should_entity_components_returns_not_one_same_component()
        {
            var fakeEntityId = 1;
            var components = new IComponent[] {new TestComponentOne(), new TestComponentTwo(), new TestComponentThree()};
            var componentDatabase = Substitute.For<IComponentDatabase>();
            componentDatabase.Allocate(Arg.Any<int>()).Returns(0);
            componentDatabase.Get<IComponent>(Arg.Any<int>(), Arg.Any<int>()).Returns(info => components[info.ArgAt<int>(0)]);
                
            var componentTypeLookup = Substitute.For<IComponentTypeLookup>();
            componentTypeLookup.AllComponentTypeIds.Returns(new[] {0, 1, 2});
            componentTypeLookup.GetComponentType(components[0].GetType()).Returns(0);
            componentTypeLookup.GetComponentType(components[1].GetType()).Returns(1);
            componentTypeLookup.GetComponentType(components[2].GetType()).Returns(2);
            
            var entity = new Entity(fakeEntityId, componentDatabase, componentTypeLookup);
            entity.AddComponents(components);
            Assert.Equal(components, entity.Components);
        }
    }
}
