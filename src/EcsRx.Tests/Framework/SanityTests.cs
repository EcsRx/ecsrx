using System;
using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.Systems.Handlers;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class SanityTests
    {
        private IEntityCollectionManager CreateCollectionManager()
        {
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var componentRepository = new ComponentRepository(componentLookupType, componentDatabase);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentRepository);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory();
            return new EntityCollectionManager(collectionFactory, observableGroupFactory);
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

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityTwo.AddComponents(new TestComponentTwo());

            Assert.Equal("woop", entityOne.GetComponent<TestComponentOne>().Data);
            Assert.Null(entityTwo.GetComponent<TestComponentTwo>().Data);
        }
        
        [Fact]
        public void should_not_freak_out_when_removing_components_during_removing_event()
        {
            var collectionManager = CreateCollectionManager();
            var collection = collectionManager.GetCollection();
            var entityOne = collection.CreateEntity();

            var timesCalled = 0;
            entityOne.ComponentsRemoved.Subscribe(x =>
            {
                entityOne.RemoveComponent<TestComponentTwo>();
                timesCalled++;
            });

            entityOne.AddComponents(new TestComponentOne(), new TestComponentTwo());
            entityOne.RemoveComponent<TestComponentOne>();
            
            Assert.Equal(2, timesCalled);
        }
    }
}