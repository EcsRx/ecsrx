using System;
using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using SystemsRx.MicroRx.Subjects;
using EcsRx.Systems;
using EcsRx.Systems.Handlers;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Plugins.ReactiveSystems.Handlers
{
    public class TeardownSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var teardownSystemHandler = new TeardownSystemHandler(observableGroupManager);
            
            var fakeMatchingSystem = Substitute.For<ITeardownSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(teardownSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_teardown_entity_when_removed()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            var fakeEntities = new List<IEntity>();

            var removeSubject = new Subject<IEntity>();
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoving.Returns(removeSubject);
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var observableGroupManager = Substitute.For<IObservableGroupManager>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            observableGroupManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ITeardownSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new TeardownSystemHandler(observableGroupManager);
            systemHandler.SetupSystem(mockSystem);
            
            removeSubject.OnNext(fakeEntity1);
            
            mockSystem.Received(1).Teardown(Arg.Is(fakeEntity1));
            Assert.Equal(1, systemHandler.SystemSubscriptions.Count);
            Assert.NotNull(systemHandler.SystemSubscriptions[mockSystem]);
        }
    }
}