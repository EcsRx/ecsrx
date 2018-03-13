using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Observable;
using EcsRx.Reactive;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class SanityTests
    {
        private IEntityCollectionManager CreateCollectionManager()
        {
            var messageBroker = new EventSystem(new MessageBroker());
            var entityFactory = new DefaultEntityFactory(messageBroker);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory, messageBroker);
            var groupAccessorFactory = new DefaultObservableObservableGroupFactory(messageBroker);
            return new EntityCollectionManager(messageBroker, collectionFactory, groupAccessorFactory);
        }
        
        private SystemExecutor CreateExecutor(IEntityCollectionManager entityCollectionManager)
        {
            var reactsToEntityHandler = new ReactToEntitySystemHandler(entityCollectionManager);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(entityCollectionManager);
            var reactsToDataHandler = new ReactToDataSystemHandler(entityCollectionManager);
            var manualSystemHandler = new ManualSystemHandler(entityCollectionManager);
            var setupHandler = new SetupSystemHandler(entityCollectionManager);

            var conventionalSystems = new List<IConventionalSystemHandler>
            {
                setupHandler,
                reactsToEntityHandler,
                reactsToGroupHandler,
                reactsToDataHandler,
                manualSystemHandler
            };
            
            return new SystemExecutor(conventionalSystems);
        }

        [Fact]
        public void should_execute_setup_for_matching_entities()
        {
            var collectionManager = CreateCollectionManager();
            var executor = CreateExecutor(collectionManager);
            executor.AddSystem(new TestSetupSystem());

            var collection = collectionManager.GetCollection();
            var entityOne = collection.CreateEntity();
            var entityTwo = collection.CreateEntity();

            entityOne.AddComponent(new TestComponentOne());
            entityTwo.AddComponent(new TestComponentTwo());

            Assert.Equal("woop", entityOne.GetComponent<TestComponentOne>().Data);
            Assert.Null(entityTwo.GetComponent<TestComponentTwo>().Data);
        }
    }
}