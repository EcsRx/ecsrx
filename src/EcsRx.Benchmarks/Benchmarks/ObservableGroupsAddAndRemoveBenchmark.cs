using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EcsRx.Collections.Entity;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using SystemsRx.Extensions;

namespace EcsRx.Benchmarks.Benchmarks
{
    [BenchmarkCategory("Groups")]
    public class ObservableGroupsAddAndRemoveBenchmark : EcsRxBenchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1000)]
        public int EntityCount;
        
        [Params(10, 20, 50)]
        public int ComponentCount;

        private IEntityCollection _collection;

        public ObservableGroupsAddAndRemoveBenchmark() : base()
        {
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
        }

        public override void Setup()
        {
            var group = new Group(_availableComponentTypes.Take(ComponentCount).ToArray());
            _collection = EntityDatabase.GetCollection();
            ObservableGroupManager.GetObservableGroup(group);
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .Take(ComponentCount)
                .ToArray();
        }

        public override void Cleanup()
        {
            _collection.RemoveAllEntities();
        }

        [Benchmark]
        public void ObservableGroupAddRemove()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveAllComponents();
            }
        }
    }
}