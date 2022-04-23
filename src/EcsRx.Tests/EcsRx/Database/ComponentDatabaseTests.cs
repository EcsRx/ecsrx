using System;
using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Database
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
            mockComponentLookup.GetComponentTypeMappings().Returns(fakeComponentTypes);
            
            var database = new ComponentDatabase(mockComponentLookup, expectedSize);           
            Assert.Equal(fakeComponentTypes.Count, database.ComponentData.Length);
            Assert.Equal(expectedSize, database.ComponentData[0].Count);
        }
        
        [Fact]
        public void should_correctly_allocate_instance_when_adding()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });
            var database = new ComponentDatabase(mockComponentLookup);
            var allocation = database.Allocate(0);
            
            Assert.Equal(0, allocation);
        }
        
        [Fact]
        public void should_correctly_remove_instance()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IComponentPool>();
            
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            database.Remove(0, 0);
            
            mockExpandingArray.Received(1).Release(0);
        }
        
        [Fact]
        public void should_dispose_component_when_removed()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentTypeMappings().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IComponentPool>();
            
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            database.Remove(0, 0);
            
            mockExpandingArray.Received(1).Release(0);
        }
    }
}