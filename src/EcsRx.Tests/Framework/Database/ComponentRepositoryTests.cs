using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework.Database
{
    public class ComponentRepositoryTests
    {
        [Fact]
        public void should_expand_database_if_needed()
        {
            var largerEntityId = 10;
            var smallerEntityId = 2;
            var defaultExpansionSize = 10;
            var expectedNewSize = largerEntityId + defaultExpansionSize + 1;

            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            
            var mockDatabase = Substitute.For<IComponentDatabase>();
            mockDatabase.CurrentEntityBounds.Returns(5);
            
            var repository = new ComponentRepository(mockComponentLookup, mockDatabase, defaultExpansionSize);
            
            repository.ExpandDatabaseIfNeeded(smallerEntityId);
            mockDatabase.Received(0).AccommodateMoreEntities(Arg.Any<int>());
            
            repository.ExpandDatabaseIfNeeded(largerEntityId);
            mockDatabase.Received(1).AccommodateMoreEntities(expectedNewSize);
        }
        
        [Fact]
        public void should_call_get_component_via_extension()
        {
            var fakeEntityId = 1;
            var defaultExpansionSize = 10;
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            
            var mockDatabase = Substitute.For<IComponentDatabase>();
            var repository = new ComponentRepository(mockComponentLookup, mockDatabase, defaultExpansionSize);

            repository.Get(fakeEntityId, typeof(TestComponentOne));
            mockDatabase.Received(1).Get<IComponent>(0, fakeEntityId);
        }
        
        [Fact]
        public void should_call_get_component()
        {
            var fakeEntityId = 1;
            var defaultExpansionSize = 10;
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            
            var mockDatabase = Substitute.For<IComponentDatabase>();
            var repository = new ComponentRepository(mockComponentLookup, mockDatabase, defaultExpansionSize);

            repository.Get(fakeEntityId, 0);
            mockDatabase.Received(1).Get<IComponent>(0, fakeEntityId);
        }
        
        [Fact]
        public void should_call_has_component_via_extension()
        {
            var fakeEntityId = 1;
            var defaultExpansionSize = 10;
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            
            var mockDatabase = Substitute.For<IComponentDatabase>();
            var repository = new ComponentRepository(mockComponentLookup, mockDatabase, defaultExpansionSize);

            repository.Has(fakeEntityId, typeof(TestComponentOne));
            mockDatabase.Received(1).Has(0, fakeEntityId);
        }
        
        [Fact]
        public void should_call_has_component()
        {
            var fakeEntityId = 1;
            var defaultExpansionSize = 10;
            
            var mockComponentLookup = Substitute.For<IComponentTypeLookup>();
            mockComponentLookup.GetComponentType(typeof(TestComponentOne)).Returns(0);
            
            var mockDatabase = Substitute.For<IComponentDatabase>();
            var repository = new ComponentRepository(mockComponentLookup, mockDatabase, defaultExpansionSize);

            repository.Has(fakeEntityId, 0);
            mockDatabase.Received(1).Has(0, fakeEntityId);
        }
        
        // Cba adding more tests they just test pass throughts which are worthless
    }
}