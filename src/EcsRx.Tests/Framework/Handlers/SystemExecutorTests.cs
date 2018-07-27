using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class SystemExecutorTests
    {
        [Fact]
        public void should_handle_and_expose_system()
        {
            var fakeSetupSystemHandler = Substitute.For<IConventionalSystemHandler>();
            var fakeSystem = Substitute.For<ISetupSystem>();

            fakeSetupSystemHandler.CanHandleSystem(Arg.Is(fakeSystem)).Returns(true);
           
            var systemExecutor = new SystemExecutor(new[] { fakeSetupSystemHandler });
            systemExecutor.AddSystem(fakeSystem);
            
            fakeSetupSystemHandler.Received(1).SetupSystem(fakeSystem);
            Assert.Contains(fakeSystem, systemExecutor.Systems);
        }
        
        [Fact]
        public void should_handle_and_remove_system()
        {
            var fakeSetupSystemHandler = Substitute.For<IConventionalSystemHandler>();
            var fakeSystem = Substitute.For<ISetupSystem>();

            fakeSetupSystemHandler.CanHandleSystem(Arg.Is(fakeSystem)).Returns(true);
           
            var systemExecutor = new SystemExecutor(new[] { fakeSetupSystemHandler });
            systemExecutor._systems.Add(fakeSystem);
            
            systemExecutor.RemoveSystem(fakeSystem);
            
            fakeSetupSystemHandler.Received(1).DestroySystem(fakeSystem);
            Assert.Empty(systemExecutor.Systems);
        }
        
        [Fact]
        public void should_destroy_all_systems_and_handlers_when_disposed()
        {
            var fakeSetupSystemHandler1 = Substitute.For<IConventionalSystemHandler>();
            var fakeSetupSystemHandler2 = Substitute.For<IConventionalSystemHandler>();
            var fakeSystem1 = Substitute.For<ISetupSystem>();
            var fakeSystem2 = Substitute.For<ISetupSystem>();
            var fakeSystem3 = Substitute.For<ISetupSystem>();

            fakeSetupSystemHandler1.CanHandleSystem(Arg.Any<ISystem>()).Returns(true);
            fakeSetupSystemHandler2.CanHandleSystem(Arg.Any<ISystem>()).Returns(true);
           
            var systemExecutor = new SystemExecutor(new[] { fakeSetupSystemHandler1, fakeSetupSystemHandler2 });
            systemExecutor._systems.Add(fakeSystem1);
            systemExecutor._systems.Add(fakeSystem2);
            systemExecutor._systems.Add(fakeSystem3);
            
            systemExecutor.Dispose();
            
            fakeSetupSystemHandler1.Received(3).DestroySystem(Arg.Any<ISystem>());
            fakeSetupSystemHandler2.Received(3).DestroySystem(Arg.Any<ISystem>());
            fakeSetupSystemHandler1.Received(1).Dispose();
            fakeSetupSystemHandler2.Received(1).Dispose();

            Assert.Empty(systemExecutor._systems);
        }
    }
}