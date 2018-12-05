using System.Linq;
using EcsRx.Pools;
using Xunit;

namespace EcsRx.Tests.Views
{
    public class IndexPoolTests
    {
        [Fact]
        public void should_allocate_up_front_ids()
        {
            var indexPool = new IndexPool(3, 3);
            Assert.Equal(3, indexPool.AvailableIndexes.Count);
            
            var expectedIdEntries = Enumerable.Range(0, 3).ToArray();
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_correctly_for_new_indexes()
        {
            var explicitNewIndex = 5;
            var indexPool = new IndexPool(3, 3);
            indexPool.Expand(explicitNewIndex);

            Assert.Equal(explicitNewIndex+1, indexPool.AvailableIndexes.Count);
            
            var expectedIdEntries = Enumerable.Range(0, explicitNewIndex+1).ToArray();
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_expand_correctly_with_auto_expansion()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var indexPool = new IndexPool(defaultExpansionAmount, originalSize);
            indexPool.Expand();

            Assert.Equal(indexPool.AvailableIndexes.Count, expectedSize);

            var expectedIdEntries = Enumerable.Range(1, expectedSize).ToArray();
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
        
        [Fact]
        public void should_allocate_and_remove_next_available_index()
        {
            var indexPool = new IndexPool(10, 10);
            var index = indexPool.AllocateInstance();
            
            Assert.InRange(index, 0, 10);
            Assert.DoesNotContain(index, indexPool.AvailableIndexes);
        }
        
        [Fact]
        public void should_expand_and_allocate_and_remove_next_available_index_when_empty()
        {
            var defaultExpansionAmount = 30;
            var originalSize = 10;
            var expectedSize = defaultExpansionAmount + originalSize;
            var indexPool = new IndexPool(defaultExpansionAmount, originalSize);
            indexPool.Clear();
            var index = indexPool.AllocateInstance();

            var expectedIdEntries = Enumerable.Range(0, expectedSize).ToList();

            Assert.InRange(index, 0, expectedSize);
            Assert.DoesNotContain(index, indexPool.AvailableIndexes);
            Assert.All(indexPool.AvailableIndexes, x => expectedIdEntries.Contains(x));
        }
    }
}