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
            var expansionSize = 100;
            var initialSize = 100;
            
            var componentPool = new ComponentPool<TestComponentOne>(initialSize);
            Assert.Equal(componentPool.Count, initialSize);
            Assert.Equal(componentPool.Components.Length, initialSize);
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
    }
}