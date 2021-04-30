using System;
using System.Diagnostics;
using SystemsRx.Infrastucture.Extensions;
using EcsRx.Collections.Entity;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Playground.Components;
using EcsRx.Plugins.Batching.Factories;

namespace EcsRx.Examples.ExampleApps.Playground
{
    public abstract class BasicLoopApplication : EcsRxConsoleApplication
    {
        protected static readonly int EntityCount = 200000;
        protected static readonly int SimulatedUpdates = 100;
        protected IEntityCollection _collection;
        protected IComponentTypeLookup _componentTypeLookup;
        protected IComponentDatabase _componentDatabase;
        protected IBatchBuilderFactory _batchBuilderFactory;
        protected IReferenceBatchBuilderFactory _referenceBatchBuilderFactory;

        protected int ClassComponent1TypeId;
        protected int ClassComponent2TypeId;
        protected int StructComponent1TypeId;
        protected int StructComponent2TypeId;
        
        protected override void ApplicationStarted()
        {
            _componentTypeLookup = Container.Resolve<IComponentTypeLookup>();
            _componentDatabase = Container.Resolve<IComponentDatabase>();
            _batchBuilderFactory = Container.Resolve<IBatchBuilderFactory>();
            _referenceBatchBuilderFactory = Container.Resolve<IReferenceBatchBuilderFactory>();
            _collection = EntityDatabase.GetCollection();

            ClassComponent1TypeId = _componentTypeLookup.GetComponentType(typeof(ClassComponent));
            ClassComponent2TypeId = _componentTypeLookup.GetComponentType(typeof(ClassComponent2));
            StructComponent1TypeId = _componentTypeLookup.GetComponentType(typeof(StructComponent));
            StructComponent2TypeId = _componentTypeLookup.GetComponentType(typeof(StructComponent2));
            
            var name = GetType().Name;
            Console.WriteLine($"{name} - {Description}");
            var timer = Stopwatch.StartNew();
            SetupEntities();
            timer.Stop();
            Console.WriteLine($"{name} - Setting up {EntityCount} entities in {timer.ElapsedMilliseconds}ms");
            
            timer.Reset();
            timer.Start();
            for(var update=0;update<SimulatedUpdates;update++)
            { RunProcess(); }
            timer.Stop();
            var totalProcessTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"{name} - Simulating {SimulatedUpdates} updates - Processing {EntityCount} entities in {totalProcessTime.TotalMilliseconds}ms");
            Console.WriteLine();
        }

        protected virtual void SetupEntities()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                SetupEntity(entity);              
            }
        }

        protected abstract string Description { get; }
        protected abstract void SetupEntity(IEntity entity);
        protected abstract void RunProcess();
    }
}