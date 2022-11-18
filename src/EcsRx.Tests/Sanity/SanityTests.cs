﻿using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Events;
using SystemsRx.Executor;
using SystemsRx.Executor.Handlers;
using SystemsRx.Executor.Handlers.Conventional;
using SystemsRx.Pools;
using SystemsRx.Threading;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Groups.Observable.Tracking;
using SystemsRx.MicroRx.Events;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.ReactiveSystems.Handlers;
using EcsRx.Plugins.Views.Components;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Tests.Models;
using EcsRx.Tests.Systems;
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

        private (IObservableGroupManager, IEntityDatabase, IComponentDatabase, IComponentTypeLookup) CreateFramework()
        {
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(ViewComponent), 3},
                {typeof(TestStructComponentOne), 4},
                {typeof(TestStructComponentTwo), 5},
                {typeof(ComponentWithReactiveProperty), 6}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentDatabase, componentLookupType);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var entityDatabase = new EntityDatabase(collectionFactory);
            var groupTrackerFactory = new GroupTrackerFactory(componentLookupType);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory(groupTrackerFactory);
            var observableGroupManager = new ObservableGroupManager(observableGroupFactory, entityDatabase, componentLookupType);

            return (observableGroupManager, entityDatabase, componentDatabase, componentLookupType);
        }

        private SystemExecutor CreateExecutor(IObservableGroupManager observableGroupManager)
        {
            var threadHandler = new DefaultThreadHandler();
            var reactsToEntityHandler = new ReactToEntitySystemHandler(observableGroupManager);
            var reactsToGroupHandler = new ReactToGroupSystemHandler(observableGroupManager, threadHandler);
            var reactsToDataHandler = new ReactToDataSystemHandler(observableGroupManager);
            var manualSystemHandler = new ManualSystemHandler();
            var setupHandler = new SetupSystemHandler(observableGroupManager);
            var teardownHandler = new TeardownSystemHandler(observableGroupManager);

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
        public void entity_disposal_works_when_using_late_initialized_observable_groups_without_matches()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();

            var collection = entityDatabase.GetCollection();
            var entityOne = collection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new TestComponentThree());

            // note, that this behaves differently for entities present before it is called.
            _ = observableGroupManager.GetObservableGroup(new Group(typeof(TestComponentOne), typeof(TestComponentTwo)));

            entityOne.Dispose(); // just asseting this doesn't throw an exception
        }

        [Fact]
        public void should_execute_setup_for_matching_entities()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            executor.AddSystem(new TestSetupSystem());

            var collection = entityDatabase.GetCollection();
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
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();
            var collection = entityDatabase.GetCollection();
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
            var observableGroupManager = Substitute.For<IObservableGroupManager>();
            var setupSystemHandler = new SetupSystemHandler(observableGroupManager);
            var teardownSystemHandler = new TeardownSystemHandler(observableGroupManager);

            var viewSystem = Substitute.For<IViewResolverSystem>();

            Assert.True(setupSystemHandler.CanHandleSystem(viewSystem));
            Assert.True(teardownSystemHandler.CanHandleSystem(viewSystem));
        }

        [Fact]
        public void should_trigger_both_setup_and_teardown_for_view_resolver()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();
            var executor = CreateExecutor(observableGroupManager);
            var viewResolverSystem = new TestViewResolverSystem(new EventSystem(new MessageBroker(), new DefaultThreadHandler()),
                new Group(typeof(TestComponentOne), typeof(ViewComponent)));
            executor.AddSystem(viewResolverSystem);

            var setupCalled = false;
            viewResolverSystem.OnSetup = entity => { setupCalled = true; };
            var teardownCalled = false;
            viewResolverSystem.OnTeardown = entity => { teardownCalled = true; };

            var collection = entityDatabase.GetCollection();
            var entityOne = collection.CreateEntity();
            entityOne.AddComponents(new TestComponentOne(), new ViewComponent());

            collection.RemoveEntity(entityOne.Id);

            Assert.True(setupCalled);
            Assert.True(teardownCalled);
        }

        [Fact]
        public void should_listen_to_multiple_collections_for_updates()
        {
            var (observableGroupManager, entityDatabase, _, _) = CreateFramework();

            var group = new Group(typeof(TestComponentOne));
            var collection1 = entityDatabase.CreateCollection(1);
            var collection2 = entityDatabase.CreateCollection(2);

            var addedTimesCalled = 0;
            var removingTimesCalled = 0;
            var removedTimesCalled = 0;
            var observableGroup = observableGroupManager.GetObservableGroup(group, 1, 2);
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
            var (_, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection1 = entityDatabase.CreateCollection(1);
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

            ref var initialBatchData = ref batch.Batches[0];
            ref var component1 = ref *initialBatchData.Component1;
            ref var component2 = ref *initialBatchData.Component2;
            Assert.Equal(startingInt, component1.Data);
            Assert.Equal(startingFloat, component2.Data);

            component1.Data = finalInt;
            component2.Data = finalFloat;

            Assert.Equal(finalInt, (*batch.Batches[0].Component1).Data);
            Assert.Equal(finalInt, structComponent1.Data);
            Assert.Equal(finalInt, componentDatabase.Get<TestStructComponentOne>(4, component1Allocation).Data);
            Assert.Equal(finalFloat, (*batch.Batches[0].Component2).Data);
            Assert.Equal(finalFloat, structComponent2.Data);
            Assert.Equal(finalFloat, componentDatabase.Get<TestStructComponentTwo>(5, component2Allocation).Data);
        }

        [Fact]
        public unsafe void should_retain_pointer_through_new_struct()
        {
            var (_, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection1 = entityDatabase.CreateCollection(1);
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

            ref var initialBatchData = ref batch.Batches[0];
            ref var component1 = ref *initialBatchData.Component1;
            ref var component2 = ref *initialBatchData.Component2;

            Assert.Equal(startingInt, component1.Data);
            Assert.Equal(startingFloat, component2.Data);

            component1 = new TestStructComponentOne {Data = finalInt};
            component2 = new TestStructComponentTwo {Data = finalFloat};

            Assert.Equal(finalInt, (*batch.Batches[0].Component1).Data);
            Assert.Equal(finalInt, structComponent1.Data);
            Assert.Equal(finalInt, componentDatabase.Get<TestStructComponentOne>(4, component1Allocation).Data);
            Assert.Equal(finalFloat, (*batch.Batches[0].Component2).Data);
            Assert.Equal(finalFloat, structComponent2.Data);
            Assert.Equal(finalFloat, componentDatabase.Get<TestStructComponentTwo>(5, component2Allocation).Data);
        }

        [Fact]
        public void should_allocate_entities_correctly()
        {
            var expectedSize = 5000;
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var observableGroup = observableGroupManager.GetObservableGroup(new Group(typeof(ViewComponent), typeof(TestComponentOne)));

            for (var i = 0; i < expectedSize; i++)
            {
                var entity = collection.CreateEntity();
                entity.AddComponents(new ViewComponent(), new TestComponentOne());
            }

            Assert.Equal(expectedSize, collection.Count);
            Assert.Equal(expectedSize, observableGroup.Count);

            var viewComponentPool = componentDatabase.GetPoolFor<ViewComponent>(componentLookup.GetComponentTypeId(typeof(ViewComponent)));
            Assert.Equal(expectedSize, viewComponentPool.Components.Length);

            var testComponentPool = componentDatabase.GetPoolFor<TestComponentOne>(componentLookup.GetComponentTypeId(typeof(TestComponentOne)));
            Assert.Equal(expectedSize, testComponentPool.Components.Length);
        }
        
        [Fact]
        public void should_handle_deletion_while_setting_up_in_reactive_data_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var executor = CreateExecutor(observableGroupManager);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveDataTestSystem1(collection);
            var systemB = new DeletingReactiveDataTestSystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);

            var reactiveDataSystemsHandler = (ReactToDataSystemHandler)executor._conventionalSystemHandlers.Single(x => x is ReactToDataSystemHandler);
            Assert.Equal(2, reactiveDataSystemsHandler.EntitySubscriptions.Count);
            Assert.Empty(reactiveDataSystemsHandler.EntitySubscriptions[systemA].Values);
            Assert.Empty(reactiveDataSystemsHandler.EntitySubscriptions[systemB].Values);
        }
        
        [Fact]
        public void should_handle_deletion_while_setting_up_in_reactive_entity_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var executor = CreateExecutor(observableGroupManager);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingReactiveEntityTestSystem1(collection);
            var systemB = new DeletingReactiveEntityTestSystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);

            var systemHandler = (ReactToEntitySystemHandler)executor._conventionalSystemHandlers.Single(x => x is ReactToEntitySystemHandler);
            Assert.Equal(2, systemHandler._entitySubscriptions.Count);
            Assert.Empty(systemHandler._entitySubscriptions[systemA].Values);
            Assert.Empty(systemHandler._entitySubscriptions[systemB].Values);
        }
        
        [Fact]
        public void should_handle_deletion_while_setting_up_in_setup_systems()
        {
            var (observableGroupManager, entityDatabase, componentDatabase, componentLookup) = CreateFramework();
            var collection = entityDatabase.GetCollection();
            var executor = CreateExecutor(observableGroupManager);

            var entity = collection.CreateEntity();
            entity.AddComponent(new ComponentWithReactiveProperty());

            var systemA = new DeletingSetupTestSystem1(collection);
            var systemB = new DeletingSetupTestSystem2();
            executor.AddSystem(systemA);
            executor.AddSystem(systemB);

            var reactiveDataSystemsHandler = (SetupSystemHandler)executor._conventionalSystemHandlers.Single(x => x is SetupSystemHandler);
            Assert.Equal(2, reactiveDataSystemsHandler._entitySubscriptions.Count);
            Assert.Empty(reactiveDataSystemsHandler._entitySubscriptions[systemA].Values);
            Assert.Empty(reactiveDataSystemsHandler._entitySubscriptions[systemB].Values);
        }
    }
}