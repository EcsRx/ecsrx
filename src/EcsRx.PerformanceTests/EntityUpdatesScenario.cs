using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.PerformanceTests.Helper;
using EcsRx.PerformanceTests.Systems;
using EcsRx.Reactive;
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

        private IEventSystem _eventSystem;

        private IReactToEntitySystem _system;
        private List<IEntity> _entities;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _eventSystem = new EventSystem(new MessageBroker());

            _availableComponents = _groupFactory.GetComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _entities = new List<IEntity>();
            for (var i = 0; i < Entities; i++)
            {
                var entity = new Entity(Guid.NewGuid(), _eventSystem);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);
            }
            
            _system = new EntityUpdateSystem();
        }

        [Benchmark]
        public void GetAllComponentsOnAllEntities()
        {
            for (var i = 0; i < Entities; i++)
            { _system.Execute(_entities[i]); }
        }
    }
}