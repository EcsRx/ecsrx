using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using EcsRx.PerformanceTests.Systems;
using EcsRx.Systems;

namespace EcsRx.PerformanceTests
{
    [Config(typeof(PerformanceConfig))]
    public class EntityUpdatesScenario
    {
        [Params(100000)]
        public int Entities;

        private IComponent[] _availableComponents;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        private IComponentRepository _componentRepository;

        private IReactToEntitySystem _system;
        private List<IEntity> _entities;

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
            _componentRepository = new ComponentRepository(componentLookup, componentDatabase);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _entities = new List<IEntity>();
            for (var i = 0; i < Entities; i++)
            {
                var entity = new Entity(i, _componentRepository);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);
            }
            
            _system = new EntityUpdateSystem();
        }

        [Benchmark]
        public void GetAllComponentsOnAllEntities()
        {
            for (var i = 0; i < Entities; i++)
            { _system.Process(_entities[i]); }
        }
    }
}