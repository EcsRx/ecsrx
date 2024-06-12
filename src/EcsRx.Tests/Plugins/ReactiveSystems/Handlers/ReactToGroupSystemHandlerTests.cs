using System;
using System.Collections.Generic;
using SystemsRx.Threading;
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
    public class ReactToGroupSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var reactToEntitySystemHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            
            var fakeMatchingSystem = Substitute.For<IReactToGroupSystem>();
            var fakeMatchingSystem2 = Substitute.For<IReactToGroupExSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem2));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var fakeEntities = new List<IEntity>
            {
                Substitute.For<IEntity>(),
                Substitute.For<IEntity>()
            };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup[0].Returns(fakeEntities[0]);
            mockObservableGroup[1].Returns(fakeEntities[1]);
            mockObservableGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntity>());
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_execute_system_without_predicate_with_pre_post()
        {
            var fakeEntities = new List<IEntity>
            {
                Substitute.For<IEntity>(),
                Substitute.For<IEntity>()
            };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            mockObservableGroup[0].Returns(fakeEntities[0]);
            mockObservableGroup[1].Returns(fakeEntities[1]);
            mockObservableGroup.Count.Returns(fakeEntities.Count);
            
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();

            var fakeGroup = Group.Empty;
            observableGroupManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupExSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntity>());
            mockSystem.ReceivedWithAnyArgs(1).BeforeProcessing();
            mockSystem.ReceivedWithAnyArgs(1).AfterProcessing();
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_only_execute_system_when_predicate_met()
        {
            var entityToMatch = Substitute.For<IEntity>();
            var idToMatch = 1;
            entityToMatch.Id.Returns(idToMatch);
            
            var fakeEntities = new List<IEntity>
            {
                entityToMatch,
                Substitute.For<IEntity>()
            };
           
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            
            var fakeGroup = new GroupWithPredicate(x => x.Id == idToMatch);
            observableGroupManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(1).Process(Arg.Is(entityToMatch));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}