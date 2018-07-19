using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class SetupSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var reactToEntitySystemHandler = new SetupSystemHandler(mockCollectionManager);
            
            var fakeMatchingSystem = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity2.Id.Returns(2);
            var fakeEntities = new List<IEntity> { fakeEntity1, fakeEntity2 };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity2));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
        }
        
        [Fact]
        public void should_execute_system_when_entity_added_without_predicate()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            fakeEntity2.Id.Returns(2);
            var fakeEntities = new List<IEntity>();
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var addingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addingSubject);
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(0).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(0).Setup(Arg.Is(fakeEntity2));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);

            addingSubject.OnNext(fakeEntity1);
            addingSubject.OnNext(fakeEntity2);
            
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity2));
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
        }
        
        [Fact]
        public void should_dispose_observables_when_entity_removed()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(1);
            var fakeEntities = new List<IEntity>();
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var removingSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(removingSubject);
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.RequiredComponents.Returns(new Type[0]);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);

            var mockDisposable = Substitute.For<IDisposable>();
            systemHandler._entitySubscriptions[mockSystem].Add(fakeEntity1.Id, mockDisposable);
            
            removingSubject.OnNext(fakeEntity1);
            
            mockDisposable.Received(1).Dispose();
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
        }
        
        [Fact]
        public void should_execute_systems_when_predicate_met()
        {
            var id1 = 1;
            var id2 = 2;
            
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new List<IEntity> { fakeEntity1, fakeEntity2 };
            fakeEntity1.Id.Returns(id1);
            fakeEntity2.Id.Returns(id2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = new Group(x => x.Id == fakeEntity1.Id);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(0).Setup(Arg.Is(fakeEntity2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(1, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].ContainsKey(fakeEntity2.Id));
        }
        
        [Fact]
        public void should_execute_systems_when_predicate_met_after_period()
        {
            var id1 = 1;
            var id2 = 2;
            var expectedDate = DateTime.Now + TimeSpan.FromSeconds(1);
            
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new List<IEntity> { fakeEntity1, fakeEntity2 };
            fakeEntity1.Id.Returns(id1);
            fakeEntity2.Id.Returns(id2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            mockObservableGroup.GetEnumerator().Returns(fakeEntities.GetEnumerator());
            
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            var fakeGroup = new Group(x => x.Id == fakeEntity1.Id && DateTime.Now >= expectedDate);
            mockCollectionManager.GetObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.Group.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].ContainsKey(fakeEntity1.Id));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].ContainsKey(fakeEntity2.Id));

            Thread.Sleep(1100);
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(0).Setup(Arg.Is(fakeEntity2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(1, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].ContainsKey(fakeEntity2.Id));
        }
    }
}