using System;
using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Events;
using EcsRx.MicroRx.Events;
using EcsRx.Pools;
using EcsRx.Systems.Handlers;
using EcsRx.Tests.Batches;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using EcsRx.Threading;
using EcsRx.Views.Components;
using EcsRx.Views.Systems;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace EcsRx.Tests.Framework
{
    public class SanityTests
    {
        private ITestOutputHelper _logger;

        public SanityTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        private (IEntityCollectionManager collectionManager, IBatchManager batchManager) CreateFramework()
        {
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(ViewComponent), 3},
                {typeof(TestStructComponentOne), 4}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentDatabase, componentLookupType);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory();
            var collectionManager = new EntityCollectionManager(collectionFactory, observableGroupFactory, componentLookupType);
            var batchManager = new BatchManager(componentLookupType, componentDatabase);

            return (collectionManager, batchManager);
        }
        
        private SystemExecutor CreateExecutor(IEntityCollectionManager entityCollectionManager)
        {
            var threadHandler = new DefaultThreadHandler();
            var reactsToEntityHandler = new ReactToEntitySystemHandler(entityCollectionManager);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(entityCollectionManager, threadHandler);
            var reactsToDataHandler = new ReactToDataSystemHandler(entityCollectionManager);
            var manualSystemHandler = new ManualSystemHandler(entityCollectionManager);
            var setupHandler = new SetupSystemHandler(entityCollectionManager);
            var teardownHandler = new TeardownSystemHandler(entityCollectionManager);

            var conventionalSystems = new List<IConventionalSystemHandler>
            {
                setupHandler,
                reactsToEntityHandler,
                reactsToGroupHandler,
                reactsToDataHandler,
                manualSystemHandler,
                teardownHandler
            };
            
            return new SystemExecutor(conventionalSystems);
        }

        [Fact]
        public void should_execute_setup_for_matching_entities()
        {
            var (collectionManager, batchManager) = CreateFramework();
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
            var (collectionManager, batchManager) = CreateFramework();
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
        
        [Fact]
        public void should_treat_view_handler_as_setup_system_and_teardown_system()
        {
            var mockEntityCollectionManager = Substitute.For<IEntityCollectionManager>();
            var setupSystemHandler = new SetupSystemHandler(mockEntityCollectionManager);
            var teardownSystemHandler = new TeardownSystemHandler(mockEntityCollectionManager);
            
            var viewSystem = Substitute.For<IViewResolverSystem>();
            
            Assert.True(setupSystemHandler.CanHandleSystem(viewSystem));
            Assert.True(teardownSystemHandler.CanHandleSystem(viewSystem));
        }
        
        [Fact]
        public void should_trigger_both_setup_and_teardown_for_view_resolver()
        {
            var (collectionManager, batchManager) = CreateFramework();
            var executor = CreateExecutor(collectionManager);
            var viewResolverSystem = new TestViewResolverSystem(new EventSystem(new MessageBroker()),
                new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            executor.AddSystem(viewResolverSystem);

            var setupCalled = false;
            viewResolverSystem.OnSetup = entity => { setupCalled = true; };
            var teardownCalled = false;
            viewResolverSystem.OnTeardown = entity => { teardownCalled = true; };
            
            var collection = collectionManager.GetCollection();
            var entityOne = collection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());

            collection.RemoveEntity(entityOne.Id);
            
            Assert.True(setupCalled);
            Assert.True(teardownCalled);
        }
        
        [Fact]
        public void should_listen_to_multiple_collections_for_updates()
        {
            var (collectionManager, batchManager) = CreateFramework();
            
            var group = new Group(typeof(TestComponentOne));
            var collection1 = collectionManager.CreateCollection(1);
            var collection2 = collectionManager.CreateCollection(2);

            var addedTimesCalled = 0;
            var removingTimesCalled = 0;
            var removedTimesCalled = 0;
            var observableGroup = collectionManager.GetObservableGroup(group, 1, 2);
            observableGroup.OnEntityAdded.Subscribe(x => addedTimesCalled++);
            observableGroup.OnEntityRemoving.Subscribe(x => removingTimesCalled++);
            observableGroup.OnEntityRemoved.Subscribe(x => removedTimesCalled++);

            var entity1 = collection1.CreateEntity();
            entity1.AddComponent<TestComponentOne>();

            var entity2 = collection2.CreateEntity();
            entity2.AddComponent<TestComponentOne>();
            
            collection1.RemoveEntity(entity1.Id);
            collection2.RemoveEntity(entity2.Id);
            
            Assert.Equal(2, addedTimesCalled);
            Assert.Equal(2, removingTimesCalled);
            Assert.Equal(2, removedTimesCalled);
        }
        
        [Fact]
        public void should_keep_state_with_batches()
        {            
            var (collectionManager, batchManager) = CreateFramework();
            var collection1 = collectionManager.CreateCollection(1);
            var entity1 = collection1.CreateEntity();

            var startingString = "moo";
            var startingValue = 2;
            var intermediateString = "woop";
            var intermediateValue = 5;
            var newString = "Hello";
            var newValue = 10;
            
            var refComponent = entity1.AddComponent<TestComponentOne>();
            refComponent.Data = startingString;
            
            ref var structComponent = ref entity1.AddComponent<TestStructComponentOne>(4);
            structComponent.Data = startingValue;

            var entities = new[] {entity1};
            var batch = batchManager.GetBatch<TestBatch>();
            batch.InitializeBatches(entities);
            
            ref var initialBatchData = ref batch.Batches[0];
            Assert.Equal(startingString, initialBatchData.TestComponentOne.Data);
            Assert.Equal(startingValue, initialBatchData.StructComponentOne.Data);
            
            refComponent.Data = intermediateString;
            structComponent.Data = intermediateValue;
            
            Assert.Equal(intermediateString, initialBatchData.TestComponentOne.Data);
            Assert.NotEqual(intermediateValue, initialBatchData.StructComponentOne.Data);
            
            batch.RefreshBatches(entities);
            
            Assert.Equal(intermediateString, initialBatchData.TestComponentOne.Data);
            Assert.Equal(intermediateValue, initialBatchData.StructComponentOne.Data);

            for (var index = 0; index < batch.Batches.Length; index++)
            {
                ref var batchItem = ref batch.Batches[index];
                ref var valueComponent = ref batchItem.StructComponentOne;
                
                batchItem.TestComponentOne.Data = newString;
                valueComponent.Data = newValue;
            }

            ref var batchData = ref batch.Batches[0];
            Assert.Equal(newString, batchData.TestComponentOne.Data);
            Assert.Equal(newValue, batchData.StructComponentOne.Data);
            Assert.Equal(newString, refComponent.Data);
            Assert.Equal(newValue, structComponent.Data);
            
        }
    }
}