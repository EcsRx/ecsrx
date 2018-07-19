using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
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
            applicableSystem1.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo)));
            
            var notApplicableSystem1 = Substitute.For<ISystem>();
            notApplicableSystem1.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentThree)));

            var notApplicableSystem2 = Substitute.For<ISystem>();
            notApplicableSystem2.Group.Returns(new Group(typeof(TestComponentTwo), typeof(TestComponentThree)));
            
            // Although this wants both 1 and 2, it also needs 3, which is not within the required components so shouldnt match
            var notApplicableSystem3 = Substitute.For<ISystem>();
            notApplicableSystem3.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)));

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
        
        [Fact]
        public void should_corectly_get_matching_entities()
        {
            // easier to test with real stuff
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var componentRepository = new ComponentRepository(componentLookupType, componentDatabase);
            
            var hasOneAndTwo = new Entity(1, componentRepository);
            hasOneAndTwo.AddComponent<TestComponentOne>();
            hasOneAndTwo.AddComponent<TestComponentTwo>();
            
            var hasAllComponents = new Entity(2, componentRepository);
            hasAllComponents.AddComponent<TestComponentOne>();
            hasAllComponents.AddComponent<TestComponentTwo>();
            hasAllComponents.AddComponent<TestComponentThree>();

            var hasOneAndThree = new Entity(3, componentRepository);
            hasOneAndThree.AddComponent<TestComponentOne>();
            hasOneAndThree.AddComponent<TestComponentThree>();

            var entityGroup = new [] {hasOneAndTwo, hasAllComponents, hasOneAndThree};
            
            var matchGroup1 = new Group(typeof(TestComponentOne), typeof(TestComponentTwo));
            var matchGroup2 = new Group(null, new [] {typeof(TestComponentOne), typeof(TestComponentTwo)}, new[] {typeof(TestComponentThree)});
            var matchGroup3 = new Group(null, new Type[0], new[] {typeof(TestComponentTwo)});


            var group1Results1 = entityGroup.MatchingGroup(matchGroup1).ToArray();
            Assert.Equal(2, group1Results1.Length);
            Assert.Contains(hasOneAndTwo, group1Results1);
            Assert.Contains(hasAllComponents, group1Results1);

            var group1Results2 = entityGroup.MatchingGroup(matchGroup2).ToArray();
            Assert.Equal(1, group1Results2.Length);
            Assert.Contains(hasOneAndTwo, group1Results2);
            
            var group1Results3 = entityGroup.MatchingGroup(matchGroup3).ToArray();
            Assert.Equal(1, group1Results3.Length);
            Assert.Contains(hasOneAndThree, group1Results3);
        }

    }
}