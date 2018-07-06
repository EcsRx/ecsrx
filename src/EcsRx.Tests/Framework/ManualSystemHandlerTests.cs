using EcsRx.Collections;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ManualSystemHandlerTests
    {
        [Fact]
        public void should_correctly_handle_systems()
        {
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();
            var teardownSystemHandler = new ManualSystemHandler(mockCollectionManager);
            
            var fakeMatchingSystem = Substitute.For<IManualSystem>();
            var fakeNonMatchingSystem1 = Substitute.For<IReactToEntitySystem>();
            var fakeNonMatchingSystem2 = Substitute.For<ISystem>();
            
            Assert.True(teardownSystemHandler.CanHandleSystem(fakeMatchingSystem));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem1));
            Assert.False(teardownSystemHandler.CanHandleSystem(fakeNonMatchingSystem2));
        }
        
        [Fact]
        public void should_start_system_when_added_to_handler()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            mockCollectionManager.CreateObservableGroup(Arg.Any<IGroup>()).Returns(mockObservableGroup);
            var mockSystem = Substitute.For<IManualSystem>();

            var systemHandler = new ManualSystemHandler(mockCollectionManager);
            systemHandler.SetupSystem(mockSystem);
            
            mockSystem.Received(1).StartSystem(Arg.Is(mockObservableGroup));
        }
        
        [Fact]
        public void should_stop_system_when_added_to_handler()
        {
            var mockObservableGroup = Substitute.For<IObservableGroup>();
            var mockCollectionManager = Substitute.For<IEntityCollectionManager>();

            mockCollectionManager.CreateObservableGroup(Arg.Any<IGroup>()).Returns(mockObservableGroup);
            var mockSystem = Substitute.For<IManualSystem>();

            var systemHandler = new ManualSystemHandler(mockCollectionManager);
            systemHandler.DestroySystem(mockSystem);
            
            mockSystem.Received(1).StopSystem(Arg.Is(mockObservableGroup));
        }
    }
}