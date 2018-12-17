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

        private IEntityCollectionManager CreateCollectionManager()
        {
            var componentLookups = new Dictionary<Type, int>
            {
                {typeof(TestComponentOne), 0},
                {typeof(TestComponentTwo), 1},
                {typeof(TestComponentThree), 2},
                {typeof(ViewComponent), 3}
            };
            var componentLookupType = new ComponentTypeLookup(componentLookups);
            var componentDatabase = new ComponentDatabase(componentLookupType);
            var componentRepository = new ComponentRepository(componentLookupType, componentDatabase);
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentRepository);
            var collectionFactory = new DefaultEntityCollectionFactory(entityFactory);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory();
            return new EntityCollectionManager(collectionFactory, observableGroupFactory, componentLookupType);
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
            var collectionManager = CreateCollectionManager();
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
    }
}