using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.PerformanceTests.Helper;

namespace EcsRx.PerformanceTests
{
    [Config(typeof(PerformanceConfig))]
    public class GroupPerformanceScenario
    {
        [Params(100, 10000)]
        public int Iterations;
        
        private IComponent[] _availableComponents;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        private IEntityCollectionManager _entityCollectionManager;
        private IGroup[] _testGroups;
        private IEntityCollection _defaultEntityCollection;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var componentTypeAssigner = new DefaultComponentTypeAssigner();
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            var componentLookup = new ComponentTypeLookup(allComponents);
            
            _availableComponents = allComponents.Keys
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            var componentDatabase = new ComponentDatabase(componentLookup);
            var componentRepository = new ComponentRepository(componentLookup, componentDatabase);
            
            var entityFactory = new DefaultEntityFactory(new IdPool(), componentRepository);
            var poolFactory = new DefaultEntityCollectionFactory(entityFactory);
            var observableGroupFactory = new DefaultObservableObservableGroupFactory();
            _entityCollectionManager = new EntityCollectionManager(poolFactory, observableGroupFactory, componentLookup);
            
            _availableComponents = _groupFactory.GetComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            _testGroups = _groupFactory.CreateTestGroups().ToArray();

            foreach (var group in _testGroups)
            { _entityCollectionManager.GetObservableGroup(group); }

            _defaultEntityCollection = _entityCollectionManager.GetCollection();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            foreach (var pool in _entityCollectionManager.Collections)
            { pool.RemoveAllEntities(); }
        }

        [Benchmark]
        public void CreateEntitiesWithComponentsThenRemoveThemAll()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var entity = _defaultEntityCollection.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveComponents(_availableComponents);
            }
        }
    }
}