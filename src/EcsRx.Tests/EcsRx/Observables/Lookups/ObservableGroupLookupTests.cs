using System;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Lookups;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Observables.Lookups
{
    public class ObservableGroupLookupTests
    {
        [Fact]
        public void should_see_lookup_groups_as_equal()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyGroup2 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            Assert.True(dummyGroup1.Equals(dummyGroup2));
        }
        
        [Fact]
        public void should_generate_same_hashcode_for_lookup_groups()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyGroup2 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            Assert.Equal(dummyGroup1.GetHashCode(), dummyGroup2.GetHashCode());
        }
        
        [Fact]
        public void should_see_lookup_groups_as_different()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyGroup2 = new LookupGroup(new[] { 2 }, Array.Empty<int>());
            Assert.False(dummyGroup1.Equals(dummyGroup2));
        }
        
        [Fact]
        public void should_see_lookup_tokens_as_equal()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyToken1 = new ObservableGroupToken(dummyGroup1);
            var dummyGroup2 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyToken2 = new ObservableGroupToken(dummyGroup2);
            Assert.True(dummyToken1.Equals(dummyToken2));
        }
        
        [Fact]
        public void should_see_lookup_tokens_as_different()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyToken1 = new ObservableGroupToken(dummyGroup1);
            var dummyGroup2 = new LookupGroup(new[] { 2 }, Array.Empty<int>());
            var dummyToken2 = new ObservableGroupToken(dummyGroup2);
            Assert.False(dummyToken1.Equals(dummyToken2));
        }
        
        [Fact]
        public void should_generate_same_hashcode_for_lookup_tokens()
        {
            var dummyGroup1 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyToken1 = new ObservableGroupToken(dummyGroup1);
            var dummyGroup2 = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyToken2 = new ObservableGroupToken(dummyGroup2);
            Assert.Equal(dummyToken1.GetHashCode(), dummyToken2.GetHashCode());
        }
        
        [Fact]
        public void should_correctly_identify_has_matching_group_already()
        {
            var dummyGroup = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var checkGroup = new LookupGroup(new[] { 1 }, Array.Empty<int>());
            var dummyObservableGroupToken = new ObservableGroupToken(dummyGroup);
            var checkGroupToken = new ObservableGroupToken(checkGroup);
            var dummyObservableGroup = Substitute.For<IObservableGroup>();
            dummyObservableGroup.Token.Returns(dummyObservableGroupToken);
            
            var lookup = new ObservableGroupLookup { dummyObservableGroup };

            Assert.True(lookup.Contains(checkGroupToken));
        }
    }
}