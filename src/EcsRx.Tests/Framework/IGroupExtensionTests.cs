using System;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Tests.Models;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class IGroupExtensionTests
    {
        [Fact]
        public void should_correctly_verify_group_contains_all_required_components()
        {
            var requiredComponents = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyGroup = new Group(requiredComponents, new Type[0]);

            var dummyComponents1 = new[] {typeof(TestComponentOne)};
            var dummyComponents2 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyComponents3 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents4 = new[] {typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents5 = new[] {typeof(TestComponentThree)};
            
            Assert.False(dummyGroup.ContainsAllRequiredComponents(dummyComponents1));
            Assert.True(dummyGroup.ContainsAllRequiredComponents(dummyComponents2));
            Assert.True(dummyGroup.ContainsAllRequiredComponents(dummyComponents3));
            Assert.False(dummyGroup.ContainsAllRequiredComponents(dummyComponents4));
            Assert.False(dummyGroup.ContainsAllRequiredComponents(dummyComponents5));         
        }
        
        [Fact]
        public void should_correctly_verify_group_contains_any_required_components()
        {
            var requiredComponents = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyGroup = new Group(requiredComponents, new Type[0]);

            var dummyComponents1 = new[] {typeof(TestComponentOne)};
            var dummyComponents2 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyComponents3 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents4 = new[] {typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents5 = new[] {typeof(TestComponentThree)};
            
            Assert.True(dummyGroup.ContainsAnyRequiredComponents(dummyComponents1));
            Assert.True(dummyGroup.ContainsAnyRequiredComponents(dummyComponents2));
            Assert.True(dummyGroup.ContainsAnyRequiredComponents(dummyComponents3));
            Assert.True(dummyGroup.ContainsAnyRequiredComponents(dummyComponents4));
            Assert.False(dummyGroup.ContainsAnyRequiredComponents(dummyComponents5));
        }
        
        [Fact]
        public void should_correctly_verify_group_contains_any_excluded_components()
        {
            var excludedComponents = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyGroup = new Group(new Type[0], excludedComponents);

            var dummyComponents1 = new[] {typeof(TestComponentOne)};
            var dummyComponents2 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyComponents3 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents4 = new[] {typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents5 = new[] {typeof(TestComponentThree)};
            
            Assert.True(dummyGroup.ContainsAnyExcludedComponents(dummyComponents1));
            Assert.True(dummyGroup.ContainsAnyExcludedComponents(dummyComponents2));
            Assert.True(dummyGroup.ContainsAnyExcludedComponents(dummyComponents3));
            Assert.True(dummyGroup.ContainsAnyExcludedComponents(dummyComponents4));
            Assert.False(dummyGroup.ContainsAnyExcludedComponents(dummyComponents5));
        }
        
        [Fact]
        public void should_correctly_verify_group_contains_any_components()
        {
            var requiredComponents = new[] {typeof(TestComponentOne)};
            var excludedComponents = new[] {typeof(TestComponentTwo)};
            var dummyGroup = new Group(requiredComponents, excludedComponents);

            var dummyComponents1 = new[] {typeof(TestComponentOne)};
            var dummyComponents2 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo)};
            var dummyComponents3 = new[] {typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents4 = new[] {typeof(TestComponentTwo), typeof(TestComponentThree)};
            var dummyComponents5 = new[] {typeof(TestComponentThree)};
            
            Assert.True(dummyGroup.ContainsAny(dummyComponents1));
            Assert.True(dummyGroup.ContainsAny(dummyComponents2));
            Assert.True(dummyGroup.ContainsAny(dummyComponents3));
            Assert.True(dummyGroup.ContainsAny(dummyComponents4));
            Assert.False(dummyGroup.ContainsAny(dummyComponents5));
        }
    }
}