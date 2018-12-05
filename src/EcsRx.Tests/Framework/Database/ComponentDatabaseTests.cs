using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Database
{
    public class ComponentDatabaseTests
    {
        [Fact]
        public void should_correctly_initialize()
        {
            var expectedSize = 10;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            
            Assert.Equal(expectedSize, database.CurrentEntityBounds);
            Assert.Equal(fakeComponentTypes.Count, database.EntityReferenceComponents.Length);
            Assert.Equal(expectedSize, database.EntityReferenceComponents[0].Count);
            Assert.All(database.EntityReferenceComponents, x => x.All(y => y == null));
        }
        
        [Fact]
        public void should_correctly_expand_for_new_entities()
        {
            var startingSize = 10;
            var expectedSize = 20;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, startingSize);
            database.AccommodateMoreEntities(expectedSize);
            
            Assert.Equal(expectedSize, database.CurrentEntityBounds);
            Assert.Equal(fakeComponentTypes.Count, database.EntityReferenceComponents.Length);
            Assert.Equal(expectedSize, database.EntityReferenceComponents[0].Count);
            Assert.All(database.EntityReferenceComponents, x => x.All(y => y == null));
        }

        [Fact]
        public void should_correctly_add_component()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var fakeComponent = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, fakeComponent);
            
            Assert.Equal(database.EntityReferenceComponents[0][1], fakeComponent);
            var nullCount = database.EntityReferenceComponents.Sum(x => x.Count(y => y == null));
            Assert.Equal(29, nullCount);
        }
        
        [Fact]
        public void should_get_all_value_and_reference_components_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponent2 = new TestComponentThree();
            var fakeComponent3 = new TestStructComponentOne {Data = 10};
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(TestStructComponentOne), 3}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, fakeComponent1);
            database.Add(2, fakeEntityId, fakeComponent2);
            database.Add(3, fakeEntityId, fakeComponent3);

            var allComponents = database.GetAll(fakeEntityId).ToArray();
            Assert.Equal(allComponents.Length, 3);
            Assert.True(allComponents.Contains(fakeComponent1));
            Assert.True(allComponents.Contains(fakeComponent2));
            Assert.True(allComponents.Contains(fakeComponent3));
        }
        
        [Fact]
        public void should_only_get_components_for_single_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponent2 = new TestComponentThree();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, fakeComponent1);
            database.Add(2, fakeEntityId, fakeComponent2);
            database.Add(0, otherEntityId, new TestComponentOne());
            database.Add(1, otherEntityId, new TestComponentTwo());
            database.Add(2, otherEntityId, new TestComponentThree());

            var allComponents = database.GetAll(fakeEntityId).ToArray();
            Assert.Equal(allComponents.Length, 2);
            Assert.True(allComponents.Contains(fakeComponent1));
            Assert.True(allComponents.Contains(fakeComponent2));
        }
        
        [Fact]
        public void should_get_specific_components_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, fakeComponent1);
            database.Add(0, otherEntityId, new TestComponentOne());

            var component = database.Get(0, fakeEntityId);
            Assert.Equal(fakeComponent1, component);
        }
        
        [Fact]
        public void should_correctly_identify_if_component_exists_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, fakeComponent1);
            database.Add(0, otherEntityId, new TestComponentOne());
            database.Add(1, otherEntityId, new TestComponentOne());

            var hasComponent0 = database.Has(0, fakeEntityId);
            Assert.True(hasComponent0);
            
            var hasComponent1 = database.Has(1, fakeEntityId);
            Assert.False(hasComponent1);
        }
        
        [Fact]
        public void should_correctly_remove_component_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponent1 = new TestComponentOne();
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, fakeComponent1);
            database.Add(0, otherEntityId, new TestComponentOne());
            database.Add(1, otherEntityId, new TestComponentOne());

            database.Remove(0, fakeEntityId);
            Assert.False(database.Has(0, fakeEntityId));
        }
        
        [Fact]
        public void should_correctly_remove_all_components_for_entity()
        {
            var expectedSize = 10;
            var fakeEntityId = 1;
            var otherEntityId = 2;
            var fakeComponentTypes = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);
            database.Add(0, fakeEntityId, new TestComponentOne());
            database.Add(0, fakeEntityId, new TestComponentTwo());
            database.Add(0, fakeEntityId, new TestComponentThree());
            database.Add(0, otherEntityId, new TestComponentOne());
            database.Add(1, otherEntityId, new TestComponentOne());

            database.RemoveAll(fakeEntityId);
            Assert.False(database.Has(0, fakeEntityId));
            Assert.False(database.Has(1, fakeEntityId));
            Assert.False(database.Has(2, fakeEntityId));

            var allComponents = database.GetAll(fakeEntityId);
            Assert.Empty(allComponents);
        }
    }
}