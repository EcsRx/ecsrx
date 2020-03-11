using System;
using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Subjects;
using EcsRx.Scheduling;
using EcsRx.Systems;
using EcsRx.Threading;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework.Handlers
{
    public class BasicSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var reactToEntitySystemHandler = new BasicSystemHandler(mockCollectionManager, threadHandler, observableScheduler);
            
            var fakeMatchingSystem = Substitute.For<IBasicSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IManualSystem>();
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
            mockObservableGroup[0].Returns(fakeEntities[0]);
            mockObservableGroup[1].Returns(fakeEntities[1]);
            mockObservableGroup.Count.Returns(fakeEntities.Count);
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var observableSubject = new Subject<ElapsedTime>();
            observableScheduler.OnUpdate.Returns(observableSubject);
            
            var fakeGroup = new Group();
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var mockSystem = Substitute.For<IBasicSystem>();
            mockSystem.Group.Returns(fakeGroup);
            
            var systemHandler = new BasicSystemHandler(mockCollectionManager, threadHandler, observableScheduler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(new ElapsedTime());
            
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
            mockObservableGroup[0].Returns(fakeEntities[0]);
            mockObservableGroup[1].Returns(fakeEntities[1]);
            mockObservableGroup.Count.Returns(fakeEntities.Count);
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var observableSubject = new Subject<ElapsedTime>();
            observableScheduler.OnUpdate.Returns(observableSubject);
            
            var fakeGroup = new GroupWithPredicate(x => x.Id == idToMatch);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup), Arg.Any<int[]>()).Returns(mockObservableGroup);

            var mockSystem = Substitute.For<IBasicSystem>();
            mockSystem.Group.Returns(fakeGroup);
            
            var systemHandler = new BasicSystemHandler(mockCollectionManager, threadHandler, observableScheduler);
            systemHandler.SetupSystem(mockSystem);
            
            observableSubject.OnNext(new ElapsedTime());
            
            mockSystem.ReceivedWithAnyArgs(1).Process(Arg.Is(entityToMatch));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var threadHandler = Substitute.For<IThreadHandler>();
            var observableScheduler = Substitute.For<IUpdateScheduler>();
            var mockSystem = Substitute.For<IBasicSystem>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new BasicSystemHandler(mockCollectionManager, threadHandler, observableScheduler);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }
    }
}