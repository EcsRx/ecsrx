using System;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Accessors;
using EcsRx.Pools;
using EcsRx.Systems;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class SetupSystemHandlerTests
    {
        [Test]
        public void should_correctly_handle_systems()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var reactToEntitySystemHandler = new SetupSystemHandler(mockPoolManager);
            
            var fakeMatchingSystem = Substitute.For<ISetupSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.That(reactToEntitySystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.That(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem1), Is.False);
            Assert.That(reactToEntitySystemHandler.CanHandleSystem(fakeNonMatchingSystem2), Is.False);
        }
        
        [Test]
        public void should_execute_system_without_predicate()
        {
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            fakeEntity1.Id.Returns(Guid.NewGuid());
            fakeEntity2.Id.Returns(Guid.NewGuid());
            var fakeEntities = new[] { fakeEntity1, fakeEntity2 };
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            mockObservableGroup.Entities.Returns(fakeEntities);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = Substitute.For<IGroup>();
            fakeGroup.MatchesComponents.Returns(new Type[0]);
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.TargetGroup.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity2));
            Assert.That(systemHandler._systemSubscriptions.Count, Is.EqualTo(1));
            Assert.That(systemHandler._systemSubscriptions[mockSystem], Is.Not.Null);
            Assert.That(systemHandler._entitySubscriptions.Count, Is.EqualTo(1));
            Assert.That(systemHandler._entitySubscriptions[mockSystem].Count, Is.Zero);
        }
        
        [Test]
        public void should_execute_systems_when_predicate_met()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            
            var fakeEntity1 = Substitute.For<IEntity>();
            var fakeEntity2 = Substitute.For<IEntity>();
            var fakeEntities = new[] { fakeEntity1, fakeEntity2 };
            fakeEntity1.Id.Returns(guid1);
            fakeEntity2.Id.Returns(guid2);
            
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            mockObservableGroup.OnEntityAdded.Returns(new Subject<IEntity>());
            mockObservableGroup.OnEntityRemoved.Returns(new Subject<IEntity>());
            mockObservableGroup.Entities.Returns(fakeEntities);
            
            var mockPoolManager = Substitute.For<IPoolManager>();

            var fakeGroup = new Group(x => x.Id == fakeEntity1.Id);
            mockPoolManager.CreateObservableGroup(Arg.Is(fakeGroup)).Returns(mockObservableGroup);
            
            var mockSystem = Substitute.For<ISetupSystem>();
            mockSystem.TargetGroup.Returns(fakeGroup);

            var systemHandler = new SetupSystemHandler(mockPoolManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).Setup(Arg.Is(fakeEntity1));
            mockSystem.Received(0).Setup(Arg.Is(fakeEntity2));
            
            Assert.That(systemHandler._systemSubscriptions.Count, Is.EqualTo(1));
            Assert.That(systemHandler._systemSubscriptions[mockSystem], Is.Not.Null);
            Assert.That(systemHandler._entitySubscriptions.Count, Is.EqualTo(1));
            Assert.That(systemHandler._entitySubscriptions[mockSystem].Count, Is.EqualTo(1));
            Assert.That(systemHandler._entitySubscriptions[mockSystem].ContainsKey(fakeEntity2.Id));
        }
        
      
    }
}