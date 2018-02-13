using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Accessors;
using EcsRx.Pools;
using EcsRx.Reactive;
using EcsRx.Tests.Components;
using EcsRx.Tests.Systems;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class SanityTests
    {
        
        private SystemExecutor CreateExecutor()
        {
            var messageBroker = new EventSystem(new MessageBroker());
            var entityFactory = new DefaultEntityFactory(messageBroker);
            var poolFactory = new DefaultPoolFactory(entityFactory, messageBroker);
            var groupAccessorFactory = new DefaultObservableObservableGroupFactory(messageBroker);
            var poolManager = new PoolManager(messageBroker, poolFactory, groupAccessorFactory);
            
            var reactsToEntityHandler = new ReactToEntitySystemHandler(poolManager);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(poolManager);
            var reactsToDataHandler = new ReactToDataSystemHandler(poolManager);
            var manualSystemHandler = new ManualSystemHandler(poolManager);
            var setupHandler = new SetupSystemHandler(poolManager);

            var conventionalSystems = new List<IConventionalSystemHandler>
            {
                setupHandler,
                reactsToEntityHandler,
                reactsToGroupHandler,
                reactsToDataHandler,
                manualSystemHandler
            };
            
            return new SystemExecutor(poolManager, messageBroker, conventionalSystems);
        }

        [Test]
        public void should_execute_setup_for_matching_entities()
        {
            var executor = CreateExecutor();
            executor.AddSystem(new TestSetupSystem());

            var defaultPool = executor.PoolManager.GetPool();
            var entityOne = defaultPool.CreateEntity();
            var entityTwo = defaultPool.CreateEntity();

            entityOne.AddComponent(new TestComponentOne());
            entityTwo.AddComponent(new TestComponentTwo());

            Assert.That(entityOne.GetComponent<TestComponentOne>().Data, Is.EqualTo("woop"));
            Assert.That(entityTwo.GetComponent<TestComponentTwo>().Data, Is.Null);
        }
    }
}