using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using SystemsRx.Events;
using SystemsRx.Executor;
using SystemsRx.Executor.Handlers;
using SystemsRx.Executor.Handlers.Conventional;
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
using EcsRx.MicroRx.Events;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.ReactiveSystems.Handlers;
using EcsRx.Plugins.Views.Components;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Pools;
using EcsRx.ReactiveData;
using EcsRx.ReactiveData.Collections;
using EcsRx.ReactiveData.Extensions;
using EcsRx.Tests.Models;
using EcsRx.Tests.Plugins.ReactiveData.Utils;
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
                {typeof(TestStructComponentTwo), 5}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentDatabase, componentLookupType);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var entityDatabase = new EntityDatabase(collectionFactory);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory();
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
            var viewResolverSystem = new TestViewResolverSystem(new EventSystem(new MessageBroker()),
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

            var viewComponentPool = componentDatabase.GetPoolFor<ViewComponent>(componentLookup.GetComponentType(typeof(ViewComponent)));
            Assert.Equal(expectedSize, viewComponentPool.Components.Length);
            
            var testComponentPool = componentDatabase.GetPoolFor<TestComponentOne>(componentLookup.GetComponentType(typeof(TestComponentOne)));
            Assert.Equal(expectedSize, testComponentPool.Components.Length);
        }

        [Fact]
        public void should_notify_and_update_values_with_reactive_property()
        {
            var reactiveProperty = new ReactiveProperty<int>(10);
            Assert.Equal(10, reactiveProperty.Value);

            var timesEntered = 0;
            var sub = reactiveProperty.Subscribe(x =>
            {
                if(timesEntered == 0) { Assert.Equal(10, x); }
                if(timesEntered == 1) { Assert.Equal(7, x); }
                timesEntered++;
            });
            reactiveProperty.Value = 7;
            Assert.Equal(7, reactiveProperty.Value);
            Assert.Equal(2, timesEntered);
            
            sub.Dispose();
        }
        
        [Fact]
        public void should_notify_and_update_values_with_reactive_collection()
        {
            var initial = new List<int> {1, 2, 3};
            var reactiveCollection = new ReactiveCollection<int>(initial);
            Assert.Equal(3, reactiveCollection.Count);
            Assert.Equal(initial, reactiveCollection);
            
            var timesEntered = 0;
            var sub = reactiveCollection.ObserveAdd().Subscribe(x =>
            {
                if(timesEntered == 0) { Assert.Equal(6, x.Value); }
                if(timesEntered == 1) { Assert.Equal(7, x.Value); }
                timesEntered++;
            });
            
            reactiveCollection.Add(6);
            reactiveCollection.Add(7);
            initial.Add(6);
            initial.Add(7);
            Assert.Equal(initial, reactiveCollection);
            Assert.Equal(2, timesEntered);
            
            sub.Dispose();
        }
        
        [Fact]
        public void should_notify_and_update_values_with_reactive_property_from_observable()
        {
            var reactiveProperty = Observable.Return(10).ToReactiveProperty();
            Assert.Equal(10, reactiveProperty.Value);

            var timesEntered = 0;
            var sub = reactiveProperty.Subscribe(x =>
            {
                if(timesEntered == 0) { Assert.Equal(10, x); }
                if(timesEntered == 1) { Assert.Equal(7, x); }
                timesEntered++;
            });
            reactiveProperty.Value = 7;
            Assert.Equal(7, reactiveProperty.Value);
            
            sub.Dispose();
        }
    }
}