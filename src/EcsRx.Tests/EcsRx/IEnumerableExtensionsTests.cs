﻿using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Extensions;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using EcsRx.Tests.Systems.PriorityScenarios;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void should_correctly_order_priorities()
        {
            var defaultPrioritySystem = new DefaultPriorityGroupSystem();
            var higherThanDefaultPrioritySystem = new HigherThanDefaultPriorityGroupSystem();
            var lowerThanDefaultPrioritySystem = new LowerThanDefaultPriorityGroupSystem();
            var lowPrioritySystem = new LowestPriorityGroupSystem();
            var highPrioritySystem = new HighestPriorityGroupSystem();

            var systemList = new List<IGroupSystem>
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

            var applicableSystem1 = Substitute.For<IGroupSystem>();
            applicableSystem1.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo)));
            
            var notApplicableSystem1 = Substitute.For<IGroupSystem>();
            notApplicableSystem1.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentThree)));

            var notApplicableSystem2 = Substitute.For<IGroupSystem>();
            notApplicableSystem2.Group.Returns(new Group(typeof(TestComponentTwo), typeof(TestComponentThree)));
            
            // Although this wants both 1 and 2, it also needs 3, which is not within the required components so shouldnt match
            var notApplicableSystem3 = Substitute.For<IGroupSystem>();
            notApplicableSystem3.Group.Returns(new Group(typeof(TestComponentOne), typeof(TestComponentTwo), typeof(TestComponentThree)));

            var systemList = new List<IGroupSystem>
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
        public void should_correctly_get_matching_entities()
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
            
            var hasOneAndTwo = new Entity(1, componentDatabase, componentLookupType);
            hasOneAndTwo.AddComponent<TestComponentOne>();
            hasOneAndTwo.AddComponent<TestComponentTwo>();
            
            var hasAllComponents = new Entity(2, componentDatabase, componentLookupType);
            hasAllComponents.AddComponent<TestComponentOne>();
            hasAllComponents.AddComponent<TestComponentTwo>();
            hasAllComponents.AddComponent<TestComponentThree>();

            var hasOneAndThree = new Entity(3, componentDatabase, componentLookupType);
            hasOneAndThree.AddComponent<TestComponentOne>();
            hasOneAndThree.AddComponent<TestComponentThree>();

            var entityGroup = new [] {hasOneAndTwo, hasAllComponents, hasOneAndThree};
            
            var matchGroup1 = new Group(typeof(TestComponentOne), typeof(TestComponentTwo));
            var matchGroup2 = new Group(new [] {typeof(TestComponentOne), typeof(TestComponentTwo)}, new[] {typeof(TestComponentThree)});
            var matchGroup3 = new Group(new Type[0], new[] {typeof(TestComponentTwo)});


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