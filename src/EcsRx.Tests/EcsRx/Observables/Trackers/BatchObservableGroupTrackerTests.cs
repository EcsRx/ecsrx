using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Collections.Entity;
using EcsRx.Collections.Events;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Trackers;
using EcsRx.Groups.Observable.Tracking.Types;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx.Observables.Trackers
{
    public class BatchObservableGroupTrackerTests
    {
        [Fact]
        public void should_match_when_start_tracking_matching_entity()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(true);

            var groupTracker = new BatchObservableGroupTracker(lookupGroup);
            var matchingStatus = groupTracker.StartTrackingEntity(applicableEntity);
            var doesMatch = groupTracker.IsMatching(applicableEntity.Id);
            
            Assert.True(matchingStatus);
            Assert.True(doesMatch);
        }
        
        [Fact]
        public void should_not_match_when_start_tracking_matching_entity()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var unApplicableEntity = Substitute.For<IEntity>();
            unApplicableEntity.Id.Returns(1);
            unApplicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(false);

            var groupTracker = new BatchObservableGroupTracker(lookupGroup);
            var matchingStatus = groupTracker.StartTrackingEntity(unApplicableEntity);
            var doesMatch = groupTracker.IsMatching(unApplicableEntity.Id);
            
            Assert.False(matchingStatus);
            Assert.False(doesMatch);
        }
        
        [Fact]
        public void should_raise_leaving_event_when_stopping_tracking_matching_entity()
        {
            var lookupGroup = new LookupGroup(new[] { 1,2 }, Array.Empty<int>());

            var applicableEntity = Substitute.For<IEntity>();
            applicableEntity.Id.Returns(1);
            applicableEntity.HasComponent(Arg.Is<int>(x => lookupGroup.RequiredComponents.Contains(x))).Returns(true);

            var actualEventData = new List<EntityGroupStateChanged>();
            var groupTracker = new BatchObservableGroupTracker(lookupGroup);
            groupTracker.GroupMatchingChanged.Subscribe(x => actualEventData.Add(x));
            groupTracker.StartTrackingEntity(applicableEntity);
            groupTracker.StopTrackingEntity(applicableEntity);
            
            Assert.Equal(2, actualEventData.Count);
            Assert.Equal(applicableEntity, actualEventData[0].Entity);
            Assert.Equal(GroupActionType.LeavingGroup, actualEventData[0].GroupActionType);
            Assert.Equal(applicableEntity, actualEventData[1].Entity);
            Assert.Equal(GroupActionType.LeftGroup, actualEventData[1].GroupActionType);
        }
    }
}