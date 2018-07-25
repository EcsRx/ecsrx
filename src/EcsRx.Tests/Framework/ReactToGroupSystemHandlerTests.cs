using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using EcsRx.Systems.Handlers;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ReactToGroupSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var reactToEntitySystemHandler = new ReactToGroupSystemHandler(mockCollectionManager);
            
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
            var fakeEntities = new List<IEntity>
            {
                Substitute.For<IEntity>(),
                Substitute.For<IEntity>()
            };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = new Group();
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(2).Process(Arg.Any<IEntity>());
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
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = new Group(x => x.Id == idToMatch);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var observableSubject = new Subject<IObservableGroup>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            mockSystem.Group.Returns(fakeGroup);
            mockSystem.ReactToGroup(Arg.Is(mockObservableGroup)).Returns(observableSubject);
            
            var systemHandler = new ReactToGroupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(mockObservableGroup);
            
            mockSystem.ReceivedWithAnyArgs(1).Process(Arg.Is(entityToMatch));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToGroupSystemHandler(mockCollectionManager);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}