using System.Linq;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.Collections;
using EcsRx.Plugins.GroupBinding.Systems.Handlers;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using EcsRx.Tests.SystemsRx.Handlers.Helpers;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Plugins.GroupBinding.Handlers
{
    public class GroupBindingSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            
            var fakeMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeMatchingSystem2 = Substitute.For<IReactToEntitySystem>();
            var fakeMatchingSystem3 = Substitute.For<IGroupSystem>();
            var fakeMatchingSystem4 = Substitute.For<IManualSystem>();
            var fakeMatchingSystem5 = Substitute.For<ISystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem1));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem2));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem3));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem4));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem5));
        }
        
        [Fact]
        public void should_correctly_get_applicable_properties()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var applicableProperties = reactToEntitySystemHandler.GetApplicableProperties(dummySystem.GetType());
            Assert.Equal(2, applicableProperties.Length);
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupA));
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupB));
        }

        [Fact]
        public void should_correctly_get_applicable_fields()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var applicableProperties = reactToEntitySystemHandler.GetApplicableFields(dummySystem.GetType());
            Assert.Equal(1, applicableProperties.Length);
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupC));
        }
        
        [Fact]
        public void should_get_group_from_members()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var member1 = dummySystem.GetType().GetProperty(nameof(dummySystem.ObservableGroupA));
            var member2 = dummySystem.GetType().GetProperty(nameof(dummySystem.ObservableGroupB));
            var member3 = dummySystem.GetType().GetField(nameof(dummySystem.ObservableGroupC));

            var group1 = reactToEntitySystemHandler.GetGroupAttributeIfAvailable(dummySystem, member1);
            var group2 = reactToEntitySystemHandler.GetGroupAttributeIfAvailable(dummySystem, member2);
            var group3 = reactToEntitySystemHandler.GetGroupAttributeIfAvailable(dummySystem, member3);
            
            Assert.Single(group1.RequiredComponents);
            Assert.Contains(group1.RequiredComponents, x => x == typeof(TestComponentOne));
            Assert.Single(group2.RequiredComponents);
            Assert.Contains(group2.RequiredComponents, x => x == typeof(TestComponentTwo));
            Assert.Single(group3.RequiredComponents);
            Assert.Contains(group3.RequiredComponents, x => x == typeof(TestComponentOne));
        }

        [Fact]
        public void should_throw_exception_when_no_group_system_interface_with_empty_group()
        {
            Assert.True(false);
        }
        
        [Fact]
        public void should_populate_properties()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var reactToEntitySystemHandler = new GroupBindingSystemHandler(observableGroupManager);
            var dummySystem = new SystemWithAutoGroupPopulation();

            var applicableProperties = reactToEntitySystemHandler.GetApplicableFields(dummySystem.GetType());
            Assert.Equal(1, applicableProperties.Length);
            Assert.Contains(applicableProperties, x => x.Name == nameof(dummySystem.ObservableGroupC));
            
            Assert.True(false);
        }
    }
}