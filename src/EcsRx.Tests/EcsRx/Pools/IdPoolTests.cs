using System.Collections.Generic;
using System.Linq;
using SystemsRx.Pools;
using Xunit;

namespace EcsRx.Tests.EcsRx.Pools
{
    public class IdPoolTests
    {
        [Fact]
        public void should_allocate_up_front_ids()
        {
            var idPool = new IdPool(10, 10);
            Assert.Equal(idPool.AvailableIds.Count, 10);
        }
        
        [Fact]
        public void should_expand_correctly_for_explicit_id()
        {
            var explicitNewId = 30;
            var idPool = new IdPool(10, 10);
            idPool.Expand(explicitNewId);

            Assert.Equal(idPool.AvailableIds.Count, explicitNewId);

            var expectedIdEntries = Enumerable.Range(1, explicitNewId - 1).ToArray();
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_correctly_with_auto_expansion()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var idPool = new IdPool(defaultExpansionAmount, originalSize);
            idPool.Expand();

            Assert.Equal(idPool.AvailableIds.Count, expectedSize);

            var expectedIdEntries = Enumerable.Range(1, expectedSize-1).ToArray();
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_allocate_and_remove_next_available_id()
        {
            var idPool = new IdPool(10, 10);
            var id = idPool.AllocateInstance();
            
            Assert.InRange(id, 1, 10);
            Assert.DoesNotContain(id, idPool.AvailableIds);
        }
        
        [Fact]
        public void should_expand_and_allocate_and_remove_next_available_id_when_empty()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var idPool = new IdPool(defaultExpansionAmount, originalSize);
            idPool.AvailableIds.Clear();
            var id = idPool.AllocateInstance();

            var expectedIdEntries = Enumerable.Range(1, expectedSize-1).ToList();

            Assert.InRange(id, 1, expectedSize);
            Assert.DoesNotContain(id, idPool.AvailableIds);
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_and_allocate_and_remove_specific_id_when_claiming_bigger_than_available()
        {
            var explicitNewId = 30;
            var idPool = new IdPool(10, 10);
            idPool.AllocateSpecificId(explicitNewId);

            Assert.Equal(idPool.AvailableIds.Count, explicitNewId-1);

            var expectedIdEntries = Enumerable.Range(1, explicitNewId - 2).ToArray();
            Assert.All(idPool.AvailableIds, x => expectedIdEntries.Contains(x));
        }

        [Fact]
        public void should_correctly_keep_expanding_when_continually_allocating()
        {
            var expectedSize = 5000;
            var idPool = new IdPool();
            var expectedAllocations = Enumerable.Range(1, expectedSize).ToList();
            var actualAllocations = new List<int>();
            
            for (var i = 0; i < expectedSize; i++)
            {
                var allocation = idPool.AllocateInstance(); 
                actualAllocations.Add(allocation);
            }

            Assert.Equal(expectedSize, actualAllocations.Count);
            Assert.All(actualAllocations, x => expectedAllocations.Contains(x));
        }
    }
}