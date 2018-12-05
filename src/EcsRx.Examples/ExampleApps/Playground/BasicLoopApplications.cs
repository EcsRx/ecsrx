using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Examples.Application;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Pools;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class BasicClassComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public float Something { get; set; }
    }
    
    public struct BasicStructComponent : IComponent
    {
        public Vector3 Position;
        public float Something;
    }
    
    public abstract class BasicLoopApplication : EcsRxConsoleApplication
    {
        protected static readonly int EntityCount = 1000000;
        protected IEntityCollection _collection;
        protected IComponentTypeLookup _componentTypeLookup;
        protected IComponentDatabase _componentDatabase;
        
        protected override void ApplicationStarted()
        {
            _componentTypeLookup = Container.Resolve<IComponentTypeLookup>();
            _componentDatabase = Container.Resolve<IComponentDatabase>();
            _collection = EntityCollectionManager.GetCollection();

            var timer = Stopwatch.StartNew();
            SetupEntities();
            timer.Stop();
            var totalSetupTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Setting up {EntityCount} entities in {totalSetupTime}ms");
            
            timer.Reset();
            timer.Start();
            RunProcess();
            timer.Stop();
            var totalProcessTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Processing {EntityCount} entities in {totalProcessTime}ms");
        }

        protected virtual void SetupEntities()
        {
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponents(new BasicClassComponent());               
            }
        }

        protected abstract void RunProcess();
    }
    
    public class SimplestForEachLoopApplication : BasicLoopApplication
    {
        protected override void RunProcess()
        {
            foreach (var entity in _collection)
            {
                var basicComponent = entity.GetComponent<BasicClassComponent>();
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
        }
    }
    
    public class BasicForEachLoopApplication : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.AccommodateMoreEntities(EntityCount);
            base.SetupEntities();
        }

        protected override void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicClassComponent));
            foreach (var entity in _collection)
            {
                var basicComponent = (BasicClassComponent)entity.GetComponent(componentId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
        }
    }
    
    public class BasicForLoopApplication : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.AccommodateMoreEntities(EntityCount);
            base.SetupEntities();
        }
        
        protected override void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicClassComponent));
            for (var i = _collection.Count - 1; i >= 0; i--)
            {
                var entity = _collection[i];
                var basicComponent = (BasicClassComponent) entity.GetComponent(componentId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
        }
    }
    
    public class BasicStructForLoopApplication : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            _componentDatabase.AccommodateMoreEntities(EntityCount);
            
            for (var i = EntityCount - 1; i >= 0; i--)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponent<BasicStructComponent>(componentId);
            }
        }
        
        protected override void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            for (var i = _collection.Count - 1; i >= 0; i--)
            {
                var entity = _collection[i];
                var basicComponent = entity.GetComponentStruct<BasicStructComponent>(componentId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
        }
    }
    
    public class BatchedForLoopApplication : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            _componentDatabase.AccommodateMoreEntities(EntityCount);
            base.SetupEntities();
        }
        
        protected override void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicClassComponent));
            var componentLookup = _componentDatabase.GetComponents(componentId);

            for (var i = _collection.Count - 1; i >= 0; i--)
            {
                var entity = _collection[i];
                var basicComponent = componentLookup[entity.Id] as BasicClassComponent;
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
        }
    }
    
    public class BatchedStructForLoopApplication : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            _componentDatabase.AccommodateMoreEntities(EntityCount);
            
            for (var i = EntityCount - 1; i >= 0; i--)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponent<BasicStructComponent>(componentId);
            }
        }

        protected override void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            var componentLookup = _componentDatabase.GetComponentStructs<BasicStructComponent>(componentId);

            for (var i = _collection.Count - 1; i >= 0; i--)
            {
                var entity = _collection[i];
                var basicComponent = componentLookup[entity.Id];
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
        }
    }
    
    public class MultithreadedBatchedStructForLoopApplication : BasicLoopApplication
    {
        protected override void SetupEntities()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            _componentDatabase.AccommodateMoreEntities(EntityCount);
            
            for (var i = EntityCount - 1; i >= 0; i--)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponent<BasicStructComponent>(componentId);
            }
        }

        protected override void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            var componentLookup = _componentDatabase.GetComponentStructs<BasicStructComponent>(componentId);

            Parallel.For(0, _collection.Count, (index) =>
            {
                var entity = _collection[index];
                var basicComponent = componentLookup[entity.Id];
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            });
        }
    }
    
    /*
    public class BasicStructLoopApplication : EcsRxConsoleApplication
    {
        private static readonly int EntityCount = 100000;
        private IEntityCollection _collection;
        private IComponentTypeLookup _componentTypeLookup;
        
        protected override void ApplicationStarted()
        {
            _componentTypeLookup = Container.Resolve<IComponentTypeLookup>();
            _collection = EntityCollectionManager.GetCollection();
            
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponents(new BasicStructComponent());               
            }
           
            RunProcess();
        }

        private void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicStructComponent));
            var timer = Stopwatch.StartNew();
            foreach (var entity in _collection)
            {
                var basicComponent = entity.GetComponent<BasicStructComponent>(componentId);
                basicComponent.Position = Vector3.One;
                basicComponent.Something = 10;
            }
            timer.Stop();

            var totalTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Executed {EntityCount} entities in single thread in {totalTime}ms");
        }
    }*/
}