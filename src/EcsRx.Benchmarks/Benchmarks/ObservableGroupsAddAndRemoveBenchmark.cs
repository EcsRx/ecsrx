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
        private List<IEntity> _entities = new List<IEntity>();
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();

        private IGroup _group;
        private IObservableGroup _observableGroup; 
        
        [Params(1000, 10000)]
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
            _group = new Group(_availableComponentTypes.Take(ComponentCount).ToArray());
            
            _observableGroup = ObservableGroupManager.GetObservableGroup(_group);
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .Take(ComponentCount)
                .ToArray();
            
            _collection = EntityDatabase.GetCollection();

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