using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Extensions;

namespace EcsRx.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Entities")]
    public class EntityRetrievalBenchmark : EcsRxBenchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private List<IEntity> _entities = new List<IEntity>();
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000, 10000, 100000)]
        public int EntityCount;

        public EntityRetrievalBenchmark() : base()
        {
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
        }

        public void ProcessEntity(IEntity entity)
        {
            entity.GetComponent<Component1>();
            entity.GetComponent<Component2>();
            entity.GetComponent<Component3>();
            entity.GetComponent<Component4>();
            entity.GetComponent<Component5>();
            entity.GetComponent<Component6>();
            entity.GetComponent<Component7>();
            entity.GetComponent<Component8>();
            entity.GetComponent<Component9>();
            entity.GetComponent<Component10>();
            entity.GetComponent<Component11>();
            entity.GetComponent<Component12>();
            entity.GetComponent<Component13>();
            entity.GetComponent<Component14>();
            entity.GetComponent<Component15>();
            entity.GetComponent<Component16>();
            entity.GetComponent<Component17>();
            entity.GetComponent<Component18>();
            entity.GetComponent<Component19>();
            entity.GetComponent<Component20>();
        }

        public override void Setup()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, ComponentDatabase, ComponentTypeLookup);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);
            }
        }

        public override void Cleanup()
        {
            _entities.ForEach(x => x.RemoveAllComponents());
            _entities.Clear();
        }

        [Benchmark]
        public void EntitiesGetComponents()
        {
            for (var i = 0; i < EntityCount; i++)
            { ProcessEntity(_entities[i]); }
        }
        
        [Benchmark]
        public bool EntitiesHasAllComponents()
        {
            var ignore = false;
            for (var i = 0; i < EntityCount; i++)
            { ignore = _entities[i].HasAllComponents(_availableComponentTypes); }
            return ignore;
        }
        
        [Benchmark]
        public bool EntitiesHasAnyComponents()
        {
            var ignore = false;
            for (var i = 0; i < EntityCount; i++)
            { ignore = _entities[i].HasAnyComponents(_availableComponentTypes); }
            return ignore;
        }
    }
}