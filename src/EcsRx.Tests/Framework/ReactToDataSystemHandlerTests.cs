using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Accessors;
using EcsRx.Pools;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests
{
    public class ReactToDataSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var reactToEntitySystemHandler = new ReactToDataSystemHandler(mockPoolManager);
            
            var fakeMatchingSystem1 = Substitute.For<IReactToDataSystem<int>>();
            var fakeMatchingSystem2 = Substitute.For<IReactToDataSystem<DateTime>>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem1));
            Assert.True(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem2));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_execute_system_without_predicate()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new[] { fakeEntity1, fakeEntity2 };

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            fakeEntity1.Id.Returns(guid1);
            fakeEntity2.Id.Returns(guid2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.Entities.Returns(fakeEntities);
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group();
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.TargetGroup.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(1);
            secondEntitySubject.OnNext(2);
            
            mockSystem.Received(1).Execute(Arg.Is(fakeEntity1), Arg.Is(1));
            mockSystem.Received(1).Execute(Arg.Is(fakeEntity2), Arg.Is(2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_execute_system_when_entity_added_to_group()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            fakeEntity1.Id.Returns(guid1);
            fakeEntity2.Id.Returns(guid2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.Entities.Returns(new IEntity[0]);
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());

            var addedSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityAdded.Returns(addedSubject);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group();
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.TargetGroup.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);

            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(0, systemHandler._entitySubscriptions[mockSystem].Count);
            
            mockSystem.Received(0).ReactToData(Arg.Any<IEntity>());
            addedSubject.OnNext(fakeEntity1);
            addedSubject.OnNext(fakeEntity2);

            mockSystem.Received(1).ReactToData(Arg.Is(fakeEntity1));
            mockSystem.Received(1).ReactToData(Arg.Is(fakeEntity2));
            
            firstEntitySubject.OnNext(1);
            secondEntitySubject.OnNext(2);
            
            mockSystem.Received(1).Execute(Arg.Is(fakeEntity1), Arg.Is(1));
            mockSystem.Received(1).Execute(Arg.Is(fakeEntity2), Arg.Is(2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_dispose_entity_subscriptions_when_removed_from_group()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new[] { fakeEntity1, fakeEntity2 };

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            fakeEntity1.Id.Returns(guid1);
            fakeEntity2.Id.Returns(guid2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.Entities.Returns(fakeEntities);
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            
            var removedSubject = new Subject<IEntity>();
            mockObservableGroup.OnEntityRemoved.Returns(removedSubject);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group();
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.TargetGroup.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);

            removedSubject.OnNext(fakeEntity1);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(1, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_only_execute_system_when_predicate_met()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new[] { fakeEntity1, fakeEntity2 };

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            fakeEntity1.Id.Returns(guid1);
            fakeEntity2.Id.Returns(guid2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.Entities.Returns(fakeEntities);
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group(x => x.Id == guid1);
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);

            var firstEntitySubject = new Subject<int>();
            var secondEntitySubject = new Subject<int>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            mockSystem.TargetGroup.Returns(fakeGroup);
            mockSystem.ReactToData(Arg.Is(fakeEntity1)).Returns(firstEntitySubject);
            mockSystem.ReactToData(Arg.Is(fakeEntity2)).Returns(secondEntitySubject);
            
            var systemHandler = new ReactToDataSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            firstEntitySubject.OnNext(1);
            secondEntitySubject.OnNext(2);
            
            mockSystem.Received(1).Execute(Arg.Is(fakeEntity1), Arg.Is(1));
            mockSystem.Received(0).Execute(Arg.Is(fakeEntity2), Arg.Is(2));
            
            Assert.Equal(1, systemHandler._systemSubscriptions.Count);
            Assert.NotNull(systemHandler._systemSubscriptions[mockSystem]);
            
            Assert.Equal(1, systemHandler._entitySubscriptions.Count);
            Assert.Equal(2, systemHandler._entitySubscriptions[mockSystem].Count);
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid1));
            Assert.True(systemHandler._entitySubscriptions[mockSystem].Keys.Contains(guid2));
            Assert.All(systemHandler._entitySubscriptions[mockSystem].Values, Assert.NotNull);
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            
            var mockPoolManager = Substitute.For<IPoolManager>();
            var mockSystem = Substitute.For<IReactToDataSystem<int>>();
            var mockSystemDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToDataSystemHandler(mockPoolManager);
            systemHandler._systemSubscriptions.Add(mockSystem, mockSystemDisposable);
            
            var entitySubscriptions = new Dictionary<Guid, IDisposable>();
            var mockEntityDisposable1 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(guid1, mockEntityDisposable1);
            var mockEntityDisposable2 = Substitute.For<IDisposable>();
            entitySubscriptions.Add(guid2, mockEntityDisposable2);
            systemHandler._entitySubscriptions.Add(mockSystem, entitySubscriptions);
            
            systemHandler.DestroySystem(mockSystem);
            
            mockSystemDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
            
            mockEntityDisposable1.Received(1).Dispose();
            mockEntityDisposable2.Received(1).Dispose();
            Assert.Equal(0, systemHandler._entitySubscriptions.Count);
        }
    }
}