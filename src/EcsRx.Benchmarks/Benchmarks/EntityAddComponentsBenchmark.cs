using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Extensions;
using SystemsRx.Extensions;

namespace EcsRx.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Entities")]
    public class EntityAddComponentsBenchmark : EcsRxBenchmark
    {
        private Type[] _availableComponentTypes;
        private List<IEntity> _entities = new List<IEntity>();
        private IComponent[] _components;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        [Params(1000)]
        public int EntityCount;

        [Params(1, 25, 50)]
        public int ComponentCount;

        public EntityAddComponentsBenchmark()
        {
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
        }

        public override void Setup()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, ComponentDatabase, ComponentTypeLookup);
                _entities.Add(entity);
            }

            _components = _availableComponentTypes
                .Take(ComponentCount)
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public override void Cleanup()
        {
            _entities.ForEach(x => x.RemoveAllComponents());
            _entities.Clear();
        }

        [Benchmark]
        public void EntitiesBatchAddComponents()
        {
            _entities.ForEach(x => x.AddComponents(_components));
        }
        
        [Benchmark]
        public void EntitiesAddIndividualComponents()
        {
            _entities.ForEach(x => _components.ForEachRun(y => x.AddComponents(y)));
        }
    }
}