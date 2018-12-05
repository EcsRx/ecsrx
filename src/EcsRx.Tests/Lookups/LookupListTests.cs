using System.Linq;
using EcsRx.Lookups;
using Xunit;

namespace EcsRx.Tests.Lookups
{
    public class LookupListTests
    {
        [Fact]
        public void should_allow_iterating_of_values()
        {
            var lookup = new LookupList<int, float>
            {
                {10, 10},
                {25, 2},
                {16, 4}
            };
            
            Assert.Equal(3, lookup.Count);
            Assert.True(lookup.ContainsKey(10));
            Assert.True(lookup.ContainsKey(25));
            Assert.True(lookup.ContainsKey(16));
            
            Assert.Contains(lookup.Values, x => x == 10);
            Assert.Contains(lookup.Values, x => x == 2);
            Assert.Contains(lookup.Values, x => x == 4);

            for (var i = 0; i < lookup.Count; i++)
            { Assert.Contains(lookup[i], lookup.Values); }
        }
        
        [Fact]
        public void should_reuse_indexes()
        {
            var lookup = new LookupList<int, float>
            {
                {10, 10},
                {25, 2},
                {16, 4}
            };

            lookup.Remove(25);
            lookup.Add(36, 5);
            
            Assert.Equal(3, lookup.Count);
            Assert.Contains(lookup.Values, x => x == 10);
            Assert.Contains(lookup.Values, x => x == 5);
            Assert.Contains(lookup.Values, x => x == 4);
        }
        
        [Fact]
        public void should_expand_when_needed()
        {
            var lookup = new LookupList<int, float>();
            var maxSize = 100000;
            
            foreach (var next in Enumerable.Range(0, maxSize))
            { lookup.Add(next, next); }
            
            Assert.Equal(maxSize, lookup.Count);
            Assert.Equal(maxSize, lookup.InternalList.Count);
        }
        
        [Fact]
        public void should_add_to_dictionary_and_list()
        {
            var lookup = new LookupList<int, float>();
            lookup.Add(0, 10);
            
            Assert.True(lookup.Lookups.ContainsKey(0));
            Assert.True(lookup.Values.Contains(10));
            Assert.True(lookup.InternalList.Contains(10));
            Assert.Equal(1, lookup.Lookups.Count);
            Assert.Equal(1, lookup.InternalList.Count);
            Assert.Equal(1, lookup.Count);
        }
        
        [Fact]
        public void should_remove_from_dictionary_and_default_element_in_list()
        {
            var lookup = new LookupList<int, string>();
            lookup.Add(0, "hello");
            lookup.Remove(0);
            
            Assert.False(lookup.Lookups.ContainsKey(0));
            Assert.False(lookup.Values.Contains(null));
            Assert.False(lookup.Values.Contains("hello"));
            Assert.False(lookup.InternalList.Contains("hello"));
            Assert.True(lookup.InternalList.Contains(null));
            Assert.Equal(0, lookup.Lookups.Count);
            Assert.Equal(1, lookup.InternalList.Count);
            Assert.Equal(0, lookup.Count);
        }
    }
}