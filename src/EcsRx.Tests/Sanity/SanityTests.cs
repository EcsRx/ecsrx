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
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.ReactiveSystems.Handlers;
using EcsRx.Plugins.Views.Components;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Pools;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
using EcsRx.Threading;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace EcsRx.Tests.Sanity
{
    public class SanityTests
    {
        private ITestOutputHelper _logger;

        public SanityTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        private (IEntityCollectionManager, IComponentDatabase, IComponentTypeLookup) CreateFramework()
        {
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(ViewComponent), 3},
                {typeof(TestStructComponentOne), 4},
                {typeof(TestStructComponentTwo), 5}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentDatabase, componentLookupType);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory();
            var collectionManager = new EntityCollectionManager(collectionFactory, observableGroupFactory, componentLookupType);

            return (collectionManager, componentDatabase, componentLookupType);
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
            var (collectionManager, _, _) = CreateFramework();
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
            var (collectionManager, _,_) = CreateFramework();
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
            var (collectionManager, _,_) = CreateFramework();
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
            var (collectionManager, _,_) = CreateFramework();
            
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
        public unsafe void should_keep_state_with_batches()
        {            
            var (collectionManager, componentDatabase, componentLookup) = CreateFramework();
            var collection1 = collectionManager.CreateCollection(1);
            var entity1 = collection1.CreateEntity();

            var startingInt = 2;
            var finalInt = 10;
            var startingFloat = 5.0f;
            var finalFloat = 20.0f;

            ref var structComponent1 = ref entity1.AddComponent<TestStructComponentOne>(4);
            var component1Allocation = entity1.ComponentAllocations[4];
            structComponent1.Data = startingInt;
            
            ref var structComponent2 = ref entity1.AddComponent<TestStructComponentTwo>(5);
            var component2Allocation = entity1.ComponentAllocations[5];
            structComponent2.Data = startingFloat;

            var entities = new[] {entity1};
            var batchBuilder = new BatchBuilder<TestStructComponentOne, TestStructComponentTwo>(componentDatabase, componentLookup);
            var batch = batchBuilder.Build(entities);
            
            ref var initialBatchData = ref batch[0];
            ref var component1 = ref *initialBatchData.Component1;
            ref var component2 = ref *initialBatchData.Component2;
            Assert.Equal(startingInt, component1.Data);
            Assert.Equal(startingFloat, component2.Data);
            
            component1.Data = finalInt;
            component2.Data = finalFloat;
            
            Assert.Equal(finalInt, (*batch[0].Component1).Data);
            Assert.Equal(finalInt, structComponent1.Data);
            Assert.Equal(finalInt, componentDatabase.Get<TestStructComponentOne>(4, component1Allocation).Data);
            Assert.Equal(finalFloat, (*batch[0].Component2).Data);
            Assert.Equal(finalFloat, structComponent2.Data);
            Assert.Equal(finalFloat, componentDatabase.Get<TestStructComponentTwo>(5, component2Allocation).Data);        
        }
        
        [Fact]
        public unsafe void should_retain_pointer_through_new_struct()
        {            
            var (collectionManager, componentDatabase, componentLookup) = CreateFramework();
            var collection1 = collectionManager.CreateCollection(1);
            var entity1 = collection1.CreateEntity();

            var startingInt = 2;
            var finalInt = 10;
            var startingFloat = 5.0f;
            var finalFloat = 20.0f;

            ref var structComponent1 = ref entity1.AddComponent<TestStructComponentOne>(4);
            var component1Allocation = entity1.ComponentAllocations[4];
            structComponent1.Data = startingInt;
            
            ref var structComponent2 = ref entity1.AddComponent<TestStructComponentTwo>(5);
            var component2Allocation = entity1.ComponentAllocations[5];
            structComponent2.Data = startingFloat;

            var entities = new[] {entity1};
            var batchBuilder = new BatchBuilder<TestStructComponentOne, TestStructComponentTwo>(componentDatabase, componentLookup);
            var batch = batchBuilder.Build(entities);
            
            ref var initialBatchData = ref batch[0];
            ref var component1 = ref *initialBatchData.Component1;
            ref var component2 = ref *initialBatchData.Component2;

            Assert.Equal(startingInt, component1.Data);
            Assert.Equal(startingFloat, component2.Data);
            
            component1 = new TestStructComponentOne {Data = finalInt};
            component2 = new TestStructComponentTwo {Data = finalFloat};

            Assert.Equal(finalInt, (*batch[0].Component1).Data);
            Assert.Equal(finalInt, structComponent1.Data);
            Assert.Equal(finalInt, componentDatabase.Get<TestStructComponentOne>(4, component1Allocation).Data);
            Assert.Equal(finalFloat, (*batch[0].Component2).Data);
            Assert.Equal(finalFloat, structComponent2.Data);
            Assert.Equal(finalFloat, componentDatabase.Get<TestStructComponentTwo>(5, component2Allocation).Data);        
        }
        
        [Fact]
        public void should_allocate_entities_correctly()
        {
            var expectedSize = 5000;
            var (collectionManager, componentDatabase, componentLookup) = CreateFramework();
            var collection = collectionManager.GetCollection();
            var observableGroup = collectionManager.GetObservableGroup(new Group(typeof(ViewComponent), typeof(TestComponentOne)));
            
            for (var i = 0; i < expectedSize; i++)
            {
                var entity = collection.CreateEntity();
                entity.AddComponents(new ViewComponent(), new TestComponentOne());
            }

            Assert.Equal(expectedSize, collection.Count);
            Assert.Equal(expectedSize, observableGroup.Count);

            var viewComponentPool = componentDatabase.GetPoolFor<ViewComponent>(componentLookup.GetComponentType(typeof(ViewComponent)));
            Assert.Equal(expectedSize, viewComponentPool.Components.Length);
            
            var testComponentPool = componentDatabase.GetPoolFor<TestComponentOne>(componentLookup.GetComponentType(typeof(TestComponentOne)));
            Assert.Equal(expectedSize, testComponentPool.Components.Length);
        }
    }
}