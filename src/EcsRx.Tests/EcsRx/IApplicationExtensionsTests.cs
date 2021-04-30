using System.Collections.Generic;
using System.Linq;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Systems;
using EcsRx.Infrastructure;
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Systems;
using EcsRx.Tests.Systems;
using NSubstitute;
using Xunit;

using ViewApplicationExtensions =  EcsRx.Plugins.Views.Extensions.IEcsRxApplicationExtensions;


namespace EcsRx.Tests.EcsRx
{
    public class IApplicationExtensionsTests
    {
        [Fact]
        public void should_correctly_order_default_systems()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var defaultPrioritySetupSystem = new DefaultPrioritySetupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var lowPrioritySetupSystem = new LowestPrioritySetupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();
            var highPrioritySetupSystem = new HighestPrioritySetupSystem();

            var systemList = new List<ISystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem,
                defaultPrioritySetupSystem,
                lowPrioritySetupSystem,
                highPrioritySetupSystem
            };

            var mockContainer = Substitute.For<IDependencyContainer>();
            var mockApplication = Substitute.For<IEcsRxApplication>();
            mockContainer.ResolveAll(typeof(ISystem)).Returns(systemList);
            mockApplication.Container.Returns(mockContainer);

            var orderedSystems = ISystemsRxApplicationExtensions.GetAllBoundSystems(mockApplication).ToList();

            Assert.Equal(8, orderedSystems.Count);
            Assert.Equal(highPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(highPrioritySystem, orderedSystems[1]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedSystems[2]);
            Assert.True(orderedSystems[3] == defaultPrioritySetupSystem || orderedSystems[3] == defaultPrioritySystem);
            Assert.True(orderedSystems[4] == defaultPrioritySetupSystem || orderedSystems[4] == defaultPrioritySystem);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[5]);
            Assert.Equal(lowPrioritySystem, orderedSystems[6]);
            Assert.Equal(lowPrioritySetupSystem, orderedSystems[7]);
        }
        
        [Fact]
        public void should_correctly_order_reactive_systems()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var defaultPrioritySetupSystem = new DefaultPrioritySetupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var lowPrioritySetupSystem = new LowestPrioritySetupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();
            var highPrioritySetupSystem = new HighestPrioritySetupSystem();

            var systemList = new List<ISystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem,
                defaultPrioritySetupSystem,
                lowPrioritySetupSystem,
                highPrioritySetupSystem
            };

            var mockContainer = Substitute.For<IDependencyContainer>();
            var mockApplication = Substitute.For<IEcsRxApplication>();
            mockContainer.ResolveAll(typeof(ISystem)).Returns(systemList);
            mockApplication.Container.Returns(mockContainer);

            var orderedSystems = IEcsRxApplicationExtensions.GetAllBoundReactiveSystems(mockApplication).ToList();

            Assert.Equal(8, orderedSystems.Count);
            Assert.Equal(highPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(defaultPrioritySetupSystem, orderedSystems[1]);
            Assert.Equal(lowPrioritySetupSystem, orderedSystems[2]);
            Assert.Equal(highPrioritySystem, orderedSystems[3]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedSystems[4]);
            Assert.Equal(defaultPrioritySystem, orderedSystems[5]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[6]);
            Assert.Equal(lowPrioritySystem, orderedSystems[7]);
        }
        
        [Fact]
        public void should_correctly_order_view_systems()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var defaultPrioritySetupSystem = new DefaultPrioritySetupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var lowPrioritySetupSystem = new LowestPrioritySetupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();
            var highPrioritySetupSystem = new HighestPrioritySetupSystem();
            var defaultPriorityViewSystem = new DefaultPriorityViewResolverSystem();
            var highestPriorityViewSystem = new HighestPriorityViewResolverSystem();
            var lowestPriorityViewSystem = new LowestPriorityViewResolverSystem();

            var systemList = new List<ISystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem,
                defaultPrioritySetupSystem,
                lowPrioritySetupSystem,
                highPrioritySetupSystem,
                defaultPriorityViewSystem,
                highestPriorityViewSystem,
                lowestPriorityViewSystem
            };

            var mockContainer = Substitute.For<IDependencyContainer>();
            var mockApplication = Substitute.For<IEcsRxApplication>();
            mockContainer.ResolveAll(typeof(ISystem)).Returns(systemList);
            mockApplication.Container.Returns(mockContainer);

            var orderedSystems = ViewApplicationExtensions.GetAllBoundViewSystems(mockApplication).ToList();

            Assert.Equal(11, orderedSystems.Count);
            Assert.Equal(highPrioritySetupSystem, orderedSystems[0]);
            Assert.Equal(defaultPrioritySetupSystem, orderedSystems[1]);
            Assert.Equal(lowPrioritySetupSystem, orderedSystems[2]);
            Assert.Equal(highestPriorityViewSystem, orderedSystems[3]);
            Assert.Equal(defaultPriorityViewSystem, orderedSystems[4]);
            Assert.Equal(lowestPriorityViewSystem, orderedSystems[5]);
            Assert.Equal(highPrioritySystem, orderedSystems[6]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedSystems[7]);
            Assert.Equal(defaultPrioritySystem, orderedSystems[8]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedSystems[9]);
            Assert.Equal(lowPrioritySystem, orderedSystems[10]);
        }
    }
}