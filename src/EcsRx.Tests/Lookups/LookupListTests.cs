using System;
using System.Linq;
using EcsRx.Lookups;
using Xunit;
using Xunit.Abstractions;

namespace EcsRx.Tests.Lookups
{
    public class LookupListTests
    {
        private readonly ITestOutputHelper output;

        public LookupListTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
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
        public void should_compact_lookups_when_removing()
        {
            var lookup = new LookupList<int, int>();
            var removalId = 2;
            
            lookup.Add(1, 1);
            lookup.Add(removalId, 2);
            lookup.Add(3, 3);
            lookup.Add(4, 4);
            lookup.Add(5, 5);

            output.WriteLine($"Starts as [{string.Join(",", lookup.Lookups.Keys)}] [{string.Join(",", lookup.Lookups.Values)}] with [{string.Join(",", lookup.InternalList)}]");
            lookup.Remove(removalId);
            output.WriteLine($"Ends as [{string.Join(",", lookup.Lookups.Keys)}] [{string.Join(",", lookup.Lookups.Values)}] with [{string.Join(",", lookup.InternalList)}]");
            
            Assert.Equal(4, lookup.Count);
            Assert.Equal(4, lookup.InternalList.Count);
            
            Assert.DoesNotContain(lookup.InternalList, x => x == removalId);
            Assert.DoesNotContain(lookup.Lookups.Keys, x => x == removalId);
            Assert.DoesNotContain(lookup.Lookups.Values, x => x == 4);
            
            Assert.Equal(1, lookup[0]);
            Assert.Equal(3, lookup[1]);
            Assert.Equal(4, lookup[2]);
            Assert.Equal(5, lookup[3]);
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