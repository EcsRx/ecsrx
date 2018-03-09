using System;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Pools;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ReactToGroupSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var reactToEntitySystemHandler = new ReactToGroupSystemHandler(mockPoolManager);
            
            var fakeMatchingSystem = Substitute.For<IReactToGroupSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var fakeEntities = new[]
            {
                Substitute.For<IEntity>(),
                Substitute.For<IEntity>()
            };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.Entities.Returns(fakeEntities);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group();
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.TargetGroup.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Execute(Arg.Any<IEntity>());
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_only_execute_system_when_predicate_met()
        {
            var entityToMatch = Substitute.For<IEntity>();
            var guidToMatch = Guid.NewGuid();
            entityToMatch.Id.Returns(guidToMatch);
            
            var fakeEntities = new[]
            {
                entityToMatch,
                Substitute.For<IEntity>()
            };
           
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.Entities.Returns(fakeEntities);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group(x => x.Id == guidToMatch);
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.TargetGroup.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(1).Execute(Arg.Is(entityToMatch));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToGroupSystemHandler(mockPoolManager);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}