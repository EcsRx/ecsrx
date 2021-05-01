using System;
using System.Linq;
using System.Reflection;
using SystemsRx.Events;
using SystemsRx.Executor.Handlers.Conventional;
using SystemsRx.Systems.Conventional;
using EcsRx.MicroRx.Subjects;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.SystemsRx.Handlers
{
    public class ReactToEventSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var reactToEventSystemHandler = new ReactToEventSystemHandler(mockEventSystem);
            
            var fakeMatchingSystem = Substitute.For<IReactToEventSystem<int>>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<IGroupSystem>();
            
            Assert.True(reactToEventSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(reactToEventSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(reactToEventSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_destroy_and_dispose_system()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockSystem = Substitute.For<IReactToEventSystem<int>>();
            var mockDisposable = Substitute.For<IDisposable>();
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            systemHandler._systemSubscriptions.Add(mockSystem, mockDisposable);
            systemHandler.DestroySystem(mockSystem);
            
            mockDisposable.Received(1).Dispose();
            Assert.Equal(0, systemHandler._systemSubscriptions.Count);
        }

        [Fact]
        public void should_get_event_type_from_system()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockSystem = Substitute.For<IReactToEventSystem<int>>();
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            var actualType = systemHandler.GetEventTypeFromSystem(mockSystem);
            
            Assert.Equal(typeof(int), actualType);
        }

        [Fact]
        public void should_process_event_when_triggered()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var dummySubject = new Subject<ComplexObject>();
            mockEventSystem.Receive<ComplexObject>().Returns(dummySubject);
            
            var mockSystem = Substitute.For<IReactToEventSystem<ComplexObject>>();
            var dummyObject = new ComplexObject(10, "Bob");
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            systemHandler.SetupSystem(mockSystem);
            dummySubject.OnNext(dummyObject);
            
            mockSystem.Received(1).Process(Arg.Is(dummyObject));
        }
    }
}