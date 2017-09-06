using System;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Accessors;
using EcsRx.Pools;
using EcsRx.Systems;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class ReactToGroupSystemHandlerTests
    {
        [Test]
        public void should_return_valid_subscription_token_when_processing()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var mockSystem = Substitute.For<IReactToGroupSystem>();
            var mockSubscription = Substitute.For<IObservable<IGroupAccessor>>();
            mockSystem.ReactToGroup(Arg.Any<IGroupAccessor>()).Returns(mockSubscription);

            var handler = new ReactToGroupSystemHandler(mockPoolManager);
            var subscriptionToken = handler.Setup(mockSystem);

            Assert.That(subscriptionToken, Is.Not.Null);
            Assert.That(subscriptionToken.AssociatedObject, Is.Null);
            Assert.That(subscriptionToken.Disposable, Is.Not.Null);
        }
    }
}