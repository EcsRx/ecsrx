using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups;
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
        public void should_return_valid_subscription_token_when_processing()
        {
            var mockEnity = Substitute.For<IEntity>();
            var mockPoolManager = Substitute.For<IPoolManager>();
            mockPoolManager.GetEntitiesFor(Arg.Any<IGroup>()).Returns(new[] {mockEnity});
            var mockSystem = Substitute.For<ISetupSystem>();

            var handler = new SetupSystemHandler(mockPoolManager);
            handler.Setup(mockSystem);

            mockSystem.Received().Setup(mockEnity);
        }
    }
}