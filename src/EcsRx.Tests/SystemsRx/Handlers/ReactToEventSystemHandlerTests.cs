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
        public void should_get_generic_event_receive()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var dummyEventType = typeof(int);
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            var methodInfo = systemHandler.GetGenericEventReceiveMethod(dummyEventType);
            
            Assert.NotNull(methodInfo);
            Assert.Equal(1, methodInfo.GetGenericArguments().Length);
            Assert.Equal(dummyEventType, methodInfo.GetGenericArguments()[0]);
        }
        
        [Fact]
        public void should_get_generic_subscription_method()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var dummyEventType = typeof(int);
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            var methodInfo = systemHandler.GetGenericSubscriptionMethod(dummyEventType);
            
            Assert.NotNull(methodInfo);
            Assert.Equal(1, methodInfo.GetGenericArguments().Length);
            Assert.Equal(dummyEventType, methodInfo.GetGenericArguments()[0]);
        }
        
        [Fact]
        public void should_get_generic_event_process_method()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var mockSystem = Substitute.For<IReactToEventSystem<int>>();
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            var methodInfo = systemHandler.GetGenericEventProcessMethod(mockSystem);
            
            Assert.NotNull(methodInfo);
            Assert.False(methodInfo.IsGenericMethod);
        }
        
        [Fact]
        public void should_get_generic_action_type()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var dummyEventType = typeof(int);
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            var genericAction = systemHandler.CreateGenericActionType(dummyEventType);
            
            Assert.NotNull(genericAction);
            Assert.Equal(typeof(Action<int>), genericAction);
        }
        
        // This is used in below test
        public static void DummyMethod(int _){}
        
        [Fact]
        public void should_get_generic_delegate()
        {
            var mockEventSystem = Substitute.For<IEventSystem>();
            var dummyActionType = typeof(Action<int>);
            var mockMethodInfo = GetType().GetMethods().First(x => x.Name == "DummyMethod");
            
            var systemHandler = new ReactToEventSystemHandler(mockEventSystem);
            var genericAction = systemHandler.CreateGenericDelegate(dummyActionType, null, mockMethodInfo);
            
            Assert.NotNull(genericAction);
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