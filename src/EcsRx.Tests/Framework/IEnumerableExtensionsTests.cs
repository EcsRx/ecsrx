using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void should_correctly_order_priorities()
        {
            var defaultPrioritySystem = new DefaultPrioritySystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPrioritySystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPrioritySystem();
            var lowPrioritySystem = new LowPrioritySystem();
            var highPrioritySystem = new HighPrioritySystem();

            var systemList = new List<ISystem>
            {
                defaultPrioritySystem,
                higherThanDefaultPrioritySystem,
                lowerThanDefaultPrioritySystem,
                lowPrioritySystem,
                highPrioritySystem
            };

            var orderedList = systemList.OrderByPriority().ToList();
            Assert.Equal(5, orderedList.Count);
            Assert.Equal(highPrioritySystem, orderedList[0]);
            Assert.Equal(higherThanDefaultPrioritySystem, orderedList[1]);
            Assert.Equal(defaultPrioritySystem, orderedList[2]);
            Assert.Equal(lowerThanDefaultPrioritySystem, orderedList[3]);
            Assert.Equal(lowPrioritySystem, orderedList[4]);
        }

        [Fact]
        public void should_correctly_get_applicable_systems()
        {
            var requiredComponents = new IComponent[] { new TestComponentOne(), new TestComponentTwo() };

            var applicableSystem1 = Substitute.For<ISystem>();
            applicableSystem1.TargetGroup.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo)));
            
            var notApplicableSystem1 = Substitute.For<ISystem>();
            notApplicableSystem1.TargetGroup.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentThree)));

            var notApplicableSystem2 = Substitute.For<ISystem>();
            notApplicableSystem2.TargetGroup.Returns(new Group(typeof(TestComponentTwo), typeof(TestComponentThree)));
            
            // Although this wants both 1 and 2, it also needs 3, which is not within the required components so shouldnt match
            var notApplicableSystem3 = Substitute.For<ISystem>();
            notApplicableSystem3.TargetGroup.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)));

            var systemList = new List<ISystem>
            {
                applicableSystem1,
                notApplicableSystem1,
                notApplicableSystem2,
                notApplicableSystem3,
            };
            
            var applicableSystems = systemList.GetApplicableSystems(requiredComponents);

            Assert.Equal(1, applicableSystems.Count());
            Assert.Contains(applicableSystem1, applicableSystems);
        }
    }
}