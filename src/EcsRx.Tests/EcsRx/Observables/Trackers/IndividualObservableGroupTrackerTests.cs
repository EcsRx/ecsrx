using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Trackers;
using EcsRx.Groups.Observable.Tracking.Types;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Observables.Trackers
{
    public class IndividualObservableGroupTrackerTests
    {
        [Theory]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        [InlineData(true, true, false)]
        public void should_correctly_match_with_is_matching(bool hasRequired, bool hasExcluding, bool shouldMatch)
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, new[] {3});

            var entityComponentAddedSub = new Subject<int[]>();
            var entityComponentRemovingSub = new Subject<int[]>();
            var entityComponentRemovedSub = new Subject<int[]>();

            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.ComponentsAdded.Returns(entityComponentAddedSub);
            entity.ComponentsRemoving.Returns(entityComponentRemovingSub);
            entity.ComponentsRemoved.Returns(entityComponentRemovedSub);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(hasRequired);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(hasExcluding);

            var timesCalled = 0;
            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new IndividualObservableGroupTracker(lookupGroup, entity);
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData.Add(x);
                timesCalled++;
            });

            var matches = groupTracker.IsMatching();
            Assert.Equal(shouldMatch, matches);
            Assert.Equal(0, timesCalled);
            Assert.Empty(actualEventData);
        }
        
        [Theory]
        [InlineData(false, false, true, false, new[]{1}, new [] {GroupActionType.JoinedGroup})]
        [InlineData(true, false, false, true, new[]{3}, new [] {GroupActionType.LeavingGroup, GroupActionType.LeftGroup})]
        [InlineData(true, false, true, true, new[]{3}, new [] {GroupActionType.LeavingGroup, GroupActionType.LeftGroup})]
        [InlineData(true, true, false, true, new[]{3}, new GroupActionType[0])]
        [InlineData(false, true, true, true, new[]{1}, new GroupActionType[0])]
        [InlineData(false, true, true, false, new[]{2}, new GroupActionType[0])]
        public void should_correctly_raise_event_for_change_on_component_addition(
            bool hasRequiredAtStart, bool hasExcludedAtStart, bool hasRequiredAfterStart, bool hasExcludedAfterStart,
            int[] componentsToChange, GroupActionType[] expectedActionTypes)
        {
            var lookupGroup = new LookupGroup(new[] { 1 }, new[] { 3 });

            var entityComponentAddedSub = new Subject<int[]>();
            var entityComponentRemovingSub = new Subject<int[]>();
            var entityComponentRemovedSub = new Subject<int[]>();

            var hasRequired = hasRequiredAtStart;
            var hasExcluded = hasExcludedAtStart;
            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.ComponentsAdded.Returns(entityComponentAddedSub);
            entity.ComponentsRemoving.Returns(entityComponentRemovingSub);
            entity.ComponentsRemoved.Returns(entityComponentRemovedSub);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(x => hasRequired);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(x => hasExcluded);

            var timesCalled = 0;
            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new IndividualObservableGroupTracker(lookupGroup, entity);
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData.Add(x);
                timesCalled++;
            });

            hasRequired = hasRequiredAfterStart;
            hasExcluded = hasExcludedAfterStart;
            entityComponentAddedSub.OnNext(componentsToChange);

            Assert.Equal(expectedActionTypes.Length, timesCalled);

            for (var i = 0; i < expectedActionTypes.Length; i++)
            { Assert.Equal(expectedActionTypes[i], actualEventData[i].GroupActionType); }
        }
        
        [Theory]
        [InlineData(true, false, false, true, new[]{1}, new [] {GroupActionType.LeavingGroup})]
        [InlineData(true, false, false, true, new[]{2}, new GroupActionType[0])]
        [InlineData(false, false, false, false, new[]{1}, new GroupActionType[0])]
        public void should_correctly_raise_event_for_change_on_component_removing(
            bool hasRequiredAtStart, bool hasExcludedAtStart, bool hasRequiredAfterStart, bool hasExcludedAfterStart,
            int[] componentsToChange, GroupActionType[] expectedActionTypes)
        {
            var lookupGroup = new LookupGroup(new[] { 1 }, new[] { 3 });

            var entityComponentAddedSub = new Subject<int[]>();
            var entityComponentRemovingSub = new Subject<int[]>();
            var entityComponentRemovedSub = new Subject<int[]>();

            var hasRequired = hasRequiredAtStart;
            var hasExcluded = hasExcludedAtStart;
            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.ComponentsAdded.Returns(entityComponentAddedSub);
            entity.ComponentsRemoving.Returns(entityComponentRemovingSub);
            entity.ComponentsRemoved.Returns(entityComponentRemovedSub);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(x => hasRequired);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(x => hasExcluded);

            var timesCalled = 0;
            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new IndividualObservableGroupTracker(lookupGroup, entity);
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData.Add(x);
                timesCalled++;
            });

            hasRequired = hasRequiredAfterStart;
            hasExcluded = hasExcludedAfterStart;
            entityComponentRemovingSub.OnNext(componentsToChange);

            Assert.Equal(expectedActionTypes.Length, timesCalled);

            for (var i = 0; i < expectedActionTypes.Length; i++)
            { Assert.Equal(expectedActionTypes[i], actualEventData[i].GroupActionType); }
        }
        
        [Theory]
        [InlineData(true, false, false, true, new[]{1}, new [] {GroupActionType.LeftGroup})]
        [InlineData(true, true, true, false, new[]{3}, new [] {GroupActionType.JoinedGroup})]
        [InlineData(true, true, true, true, new[]{2}, new GroupActionType[0])]
        [InlineData(false, false, false, false, new[]{1}, new GroupActionType[0])]
        public void should_correctly_raise_event_for_change_on_component_removed(
            bool hasRequiredAtStart, bool hasExcludedAtStart, bool hasRequiredAfterStart, bool hasExcludedAfterStart,
            int[] componentsToChange, GroupActionType[] expectedActionTypes)
        {
            var lookupGroup = new LookupGroup(new[] { 1 }, new[] { 3 });

            var entityComponentAddedSub = new Subject<int[]>();
            var entityComponentRemovingSub = new Subject<int[]>();
            var entityComponentRemovedSub = new Subject<int[]>();

            var hasRequired = hasRequiredAtStart;
            var hasExcluded = hasExcludedAtStart;
            var entity = Substitute.For<IEntity>();
            entity.Id.Returns(1);
            entity.ComponentsAdded.Returns(entityComponentAddedSub);
            entity.ComponentsRemoving.Returns(entityComponentRemovingSub);
            entity.ComponentsRemoved.Returns(entityComponentRemovedSub);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(x => hasRequired);
            entity.HasComponent(Arg.Is<int>(x => lookupGroup.ExcludedComponents.Contains(x))).Returns(x => hasExcluded);

            var timesCalled = 0;
            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new IndividualObservableGroupTracker(lookupGroup, entity);
            groupTracker.GroupMatchingChanged.Subscribe(x =>
            {
                actualEventData.Add(x);
                timesCalled++;
            });

            hasRequired = hasRequiredAfterStart;
            hasExcluded = hasExcludedAfterStart;
            entityComponentRemovedSub.OnNext(componentsToChange);

            Assert.Equal(expectedActionTypes.Length, timesCalled);

            for (var i = 0; i < expectedActionTypes.Length; i++)
            { Assert.Equal(expectedActionTypes[i], actualEventData[i].GroupActionType); }
        }
    }
}