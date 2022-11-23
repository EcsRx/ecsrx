using System;
using System.Linq;
using System.Reactive.Linq;
using BenchmarkDotNet.Attributes;
using EcsRx.Collections.Entity;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;

namespace EcsRx.Benchmarks.Benchmarks
{
    public class AddAndRemoveEntitySystem : IReactToEntitySystem
    {
        public IGroup Group { get; }

        public AddAndRemoveEntitySystem(IGroup group)
        { Group = group; }

        public IObservable<IEntity> ReactToEntity(IEntity entity)
        { return Observable.Empty(entity); }

        public void Process(IEntity entity)
        {}
    }
    
    [BenchmarkCategory("Systems", "Entities")]
    public class ExecutorAddAndRemoveEntitySystemBenchmark : EcsRxBenchmark
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        
        [Params(1, 10, 100)]
        public int SystemsToProcess;
        
        [Params(1000)]
        public int EntityCount;
        
        [Params(10, 20, 50)]
        public int ComponentCount;

        private IEntityCollection _collection;

        public ExecutorAddAndRemoveEntitySystemBenchmark() : base()
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

            for (var i = 0; i < SystemsToProcess; i++)
            {
                var system = new AddAndRemoveEntitySystem(group);
                SystemExecutor.AddSystem(system);
            }
            
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
        public void AddAndRemoveForEntitySystem()
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