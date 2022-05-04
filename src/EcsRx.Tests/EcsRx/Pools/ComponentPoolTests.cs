using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Tests.Models;
using Xunit;

namespace EcsRx.Tests.EcsRx.Pools
{
    public class ComponentPoolTests
    {
        [Fact]
        public void should_allocate_up_front_components()
        {
            var initialSize = 100;
            
            var componentPool = new ComponentPool<TestComponentOne>(initialSize);
            Assert.Equal(componentPool.Count, initialSize);
            Assert.Equal(componentPool.Components.Length, initialSize);
        }
        
        [Fact]
        public void should_correctly_identify_struct_types()
        {
            var classBasedComponentPool = new ComponentPool<TestComponentOne>(0);
            var structBasedComponentPool = new ComponentPool<TestStructComponentOne>(0);
            Assert.False(classBasedComponentPool.IsStructType);
            Assert.True(structBasedComponentPool.IsStructType);
        }
        
        [Fact]
        public void should_expand_explicitly_when_needed()
        {
            var expansionIterations = 20;
            var expansionSize = 10;
            var initialSize = 10;
            
            var componentPool = new ComponentPool<TestComponentOne>(initialSize);
            var newSize = initialSize;
            for (var i = 0; i < expansionIterations; i++)
            {
                componentPool.Expand(expansionSize);
                newSize += expansionSize;

                Assert.Equal(componentPool.Count, newSize);
                Assert.Equal(componentPool.Components.Length, newSize);
            }            
        }
        
        [Fact]
        public void should_expand_automatically_when_needed()
        {
            var expansionIterations = 20;
            var expansionSize = 10;
            var initialSize = 10;
            
            var componentPool = new ComponentPool<TestComponentOne>(expansionSize, initialSize);
            var newSize = initialSize;
            for (var i = 0; i < expansionIterations; i++)
            {
                componentPool.Expand();
                newSize += expansionSize;

                Assert.Equal(componentPool.Count, newSize);
                Assert.Equal(componentPool.Components.Length, newSize);
            }            
        }

        [Fact]
        public void should_allocate_correctly()
        {
            var initialSize = 1;
            var componentPool = new ComponentPool<TestComponentOne>(initialSize);
            var allocation = componentPool.Allocate();
            
            Assert.Equal(0, allocation);
            Assert.Equal(0, componentPool.IndexesRemaining);
        }
        
        [Fact]
        public void should_expand_for_allocations_exceeding_count_correctly()
        {
            var expectedAllocationCount = 10;
            var expansionSize = 2;
            var initialSize = 2;
            var expectedAllocations = Enumerable.Range(0, expectedAllocationCount).ToList();
            var actualAllocations = new List<int>();
            
            var componentPool = new ComponentPool<TestComponentOne>(expansionSize, initialSize);
            for (var i = 0; i < expectedAllocationCount; i++)
            {
                var allocation = componentPool.Allocate();
                actualAllocations.Add(allocation);
            }            
            
            Assert.Equal(expectedAllocationCount, actualAllocations.Count);
            Assert.All(actualAllocations, x => expectedAllocations.Contains(x));
            Assert.Equal(expectedAllocationCount, componentPool.Components.Length);
            Assert.Equal(expectedAllocationCount, componentPool.Count);
        }
        
        [Fact]
        public void should_request_index_pool_release_when_releasing_component()
        {
            var componentPool = new ComponentPool<TestStructComponentOne>(10);
            var indexToUse = componentPool.IndexPool.AllocateInstance();
            var availableIndexesPrior = componentPool.IndexPool.AvailableIndexes.ToArray();
            componentPool.Release(indexToUse);
            var availableIndexesAfter = componentPool.IndexPool.AvailableIndexes.ToArray();
            
            Assert.NotSame(availableIndexesPrior, availableIndexesAfter);
            Assert.DoesNotContain(indexToUse, availableIndexesPrior);
            Assert.Contains(indexToUse, availableIndexesAfter);
        }
        
        [Fact]
        public void should_nullify_class_based_components_on_release()
        {
            var componentPool = new ComponentPool<TestComponentOne>(10);
            var indexToUse = componentPool.IndexPool.AllocateInstance();
            componentPool.Components[indexToUse] = new TestComponentOne();
            
            componentPool.Release(indexToUse);
            
            Assert.True(componentPool.Components.All(x => x is null));
        }  
        
        [Fact]
        public void should_dispose_disposable_component_on_release()
        {
            var componentPool = new ComponentPool<TestDisposableComponent>(10);
            var indexToUse = componentPool.IndexPool.AllocateInstance();
            var componentToUse = new TestDisposableComponent();
            componentPool.Components[indexToUse] = componentToUse;
            
            componentPool.Release(indexToUse);
            
            Assert.True(componentToUse.isDisposed);
        }
    }
}