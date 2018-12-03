using System.Linq;
using EcsRx.Lookups;
using Xunit;

namespace EcsRx.Tests.Lookups
{
    public class IterableDictionaryTests
    {
        [Fact]
        public void should_allow_iterating_of_values()
        {
            var dictionary = new IterableDictionary<int, float>
            {
                {10, 10},
                {25, 2},
                {16, 4}
            };
            
            Assert.Equal(3, dictionary.Count);
            Assert.True(dictionary.ContainsKey(10));
            Assert.True(dictionary.ContainsKey(25));
            Assert.True(dictionary.ContainsKey(16));
            
            Assert.Contains(dictionary.Values, x => x == 10);
            Assert.Contains(dictionary.Values, x => x == 2);
            Assert.Contains(dictionary.Values, x => x == 4);

            for (var i = 0; i < dictionary.Count; i++)
            { Assert.Contains(dictionary[i], dictionary.Values); }
        }
        
        [Fact]
        public void should_reuse_indexes()
        {
            var dictionary = new IterableDictionary<int, float>
            {
                {10, 10},
                {25, 2},
                {16, 4}
            };

            dictionary.Remove(25);
            dictionary.Add(36, 5);
            
            Assert.Equal(3, dictionary.Count);
            Assert.Contains(dictionary.Values, x => x == 10);
            Assert.Contains(dictionary.Values, x => x == 5);
            Assert.Contains(dictionary.Values, x => x == 4);
        }
        
        [Fact]
        public void should_expand_when_needed()
        {
            var dictionary = new IterableDictionary<int, float>();
            var maxSize = 100000;
            
            foreach (var next in Enumerable.Range(0, maxSize))
            { dictionary.Add(next, next); }
            
            Assert.Equal(maxSize, dictionary.Count);
            Assert.Equal(maxSize, dictionary.InternalList.Count);
            
        }
    }
}