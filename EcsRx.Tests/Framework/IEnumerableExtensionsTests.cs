using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Components;
using EcsRx.Tests.Systems;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class IEnumerableExtensionsTests
    {
        [Test]
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
            Assert.That(orderedList, Has.Count.EqualTo(5));
            Assert.That(orderedList[0], Is.EqualTo(highPrioritySystem));
            Assert.That(orderedList[1], Is.EqualTo(higherThanDefaultPrioritySystem));
            Assert.That(orderedList[2], Is.EqualTo(defaultPrioritySystem));
            Assert.That(orderedList[3], Is.EqualTo(lowerThanDefaultPrioritySystem));
            Assert.That(orderedList[4], Is.EqualTo(lowPrioritySystem));
        }

        [Test]
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

            Assert.That(applicableSystems.Count(), Is.EqualTo(1));
            Assert.That(applicableSystems.Contains(applicableSystem1));
        }
    }
}