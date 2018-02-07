using System;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Accessors;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using EcsRx.Tests.Components;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class ReactToEntitySystemHandlerTests
    {
        [Test]
        public void should_return_valid_subscription_token_when_processing()
        {
            var mockPoolManager = Substitute.For<IPoolManager>();
            var mockEntity = Substitute.For<IEntity>();
            var mockSystem = Substitute.For<IReactToEntitySystem>();
            var mockSubscription = Substitute.For<IObservable<IEntity>>();
            mockSystem.ReactToEntity(mockEntity).Returns(mockSubscription);

            var handler = new ReactToEntitySystemHandler(mockPoolManager);
            var subscriptionToken = handler.ProcessEntity(mockSystem, mockEntity);

            Assert.That(subscriptionToken, Is.Not.Null);
            Assert.That(subscriptionToken.AssociatedObject, Is.EqualTo(mockEntity));
            Assert.That(subscriptionToken.Disposable, Is.Not.Null);
        }

        [Test]
        public void should_return_valid_subscription_collection()
        {
            var dummyGroup = new GroupBuilder().WithComponent<TestComponentOne>().Build();
            var mockEntity = Substitute.For<IEntity>();

            var mockPoolManager = Substitute.For<IPoolManager>();
            mockPoolManager.CreateObservableGroup(dummyGroup).Returns(new ObservableGroup(null, new[] {mockEntity}));

            var mockSystem = Substitute.For<IReactToEntitySystem>();
            mockSystem.TargetGroup.Returns(dummyGroup);

            var mockSubscription = Substitute.For<IObservable<IEntity>>();
            mockSystem.ReactToEntity(mockEntity).Returns(mockSubscription);

            var handler = new ReactToEntitySystemHandler(mockPoolManager);

            var subscriptionTokens = handler.Setup(mockSystem);
            Assert.That(subscriptionTokens.Count(), Is.EqualTo(1));
            Assert.That(subscriptionTokens.ElementAt(0).AssociatedObject, Is.EqualTo(mockEntity));
            Assert.That(subscriptionTokens.ElementAt(0).Disposable, Is.Not.Null);
        }
    }
}