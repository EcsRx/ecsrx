using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework.Database
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
            Assert.Equal(fakeComponentTypes.Count, database.ComponentData.Length);
            Assert.Equal(expectedSize, database.ComponentData[0].Count);
        }
        
        [Fact]
        public void should_correctly_allocate_instance_when_adding()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });
            var database = new ComponentDatabase(mockComponentLookup);
            var allocation = database.Allocate(0);
            
            Assert.Equal(0, allocation);
        }
        
        [Fact]
        public void should_correctly_set_instance()
        {
            var expectedComponent = new TestComponentOne();
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });
            var database = new ComponentDatabase(mockComponentLookup);
            database.Set(0, 0, expectedComponent);
            
            Assert.Equal(database.ComponentData[0].Get<TestComponentOne>(0), expectedComponent);
        }
        
        [Fact]
        public void should_correctly_remove_instance()
        {
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IExpandingArrayPool>();
            
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            database.Remove(0, 0);
            
            mockExpandingArray.Received(1).Release(0);
        }
        
        [Fact]
        public void should_correctly_get_instance()
        {
            var expectedComponent = new TestComponentOne();
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IExpandingArrayPool>();
            mockExpandingArray.Get<TestComponentOne>(Arg.Is(0)).Returns(expectedComponent);
                
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            var actualComponent = database.Get<TestComponentOne>(0, 0);
            
            Assert.Equal(expectedComponent, actualComponent);
        }
        
        [Fact]
        public void should_correctly_get_ref_instance()
        {
            var startingComponent = new TestStructComponentOne { Data = 10};
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(new Dictionary<Type, int>
            {
                {typeof(TestStructComponentOne), 0}
            });
            
            var database = new ComponentDatabase(mockComponentLookup);
            var underlyingStore = database.ComponentData[0];
            underlyingStore.Set(0, startingComponent);
            
            ref var actualComponent = ref database.GetRef<TestStructComponentOne>(0, 0);

            Assert.Equal(10, startingComponent.Data);
            Assert.Equal(10, underlyingStore.Get<TestStructComponentOne>(0).Data);
            Assert.Equal(10, actualComponent.Data);
            
            actualComponent.Data = 50;
            
            Assert.Equal(10, startingComponent.Data);
            Assert.Equal(50, actualComponent.Data);
            Assert.Equal(50, underlyingStore.Get<TestStructComponentOne>(0).Data);
            
            actualComponent = new TestStructComponentOne {Data = 25};
            
            Assert.Equal(10, startingComponent.Data);
            Assert.Equal(25, actualComponent.Data);
            Assert.Equal(25, underlyingStore.Get<TestStructComponentOne>(0).Data);
        }
        
        [Fact]
        public void should_correctly_get_components()
        {
            var expectedComponents = new[]
            {
                new TestComponentOne(), new TestComponentOne()
            };
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetAllComponentTypes().Returns(new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0}
            });

            var mockExpandingArray = Substitute.For<IExpandingArrayPool>();
            mockExpandingArray.AsArray<TestComponentOne>().Returns(expectedComponents);
                
            var database = new ComponentDatabase(mockComponentLookup);
            database.ComponentData.SetValue(mockExpandingArray, 0);
            var actualComponents = database.GetComponents<TestComponentOne>(0);
            
            Assert.Equal(expectedComponents, actualComponents);
        }
    }
}