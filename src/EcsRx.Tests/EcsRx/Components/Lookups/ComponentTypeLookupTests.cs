using EcsRx.Components.Lookups;
using EcsRx.Tests.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace EcsRx.Tests.EcsRx.Components.Lookups
{
    public class ComponentTypeLookupTests
    {
        [Fact]
        public void requesting_non_existing_component_type_throws_keynotfound()
        {
            var sut = new ComponentTypeLookup(new Dictionary<Type, int>());
            Assert.Throws<KeyNotFoundException>(() => sut.GetComponentTypeId(typeof(TestComponentOne)));
        }

        [Fact]
        public void requesting_existing_component_type_returns_expected_component_id()
        {
            var sut = new ComponentTypeLookup(new Dictionary<Type, int> { {typeof(ComponentWithoutInterface), 1} });
            Assert.Equal(1, sut.GetComponentTypeId(typeof(ComponentWithoutInterface)));
        }

        [Fact]
        public void requesting_non_existing_type_without_component_interface_throws_argumentexception()
        {
            var sut = new ComponentTypeLookup(new Dictionary<Type, int>());
            Assert.Throws<ArgumentException>(() => sut.GetComponentTypeId(typeof(ComponentWithoutInterface)));
        }
    }
}
