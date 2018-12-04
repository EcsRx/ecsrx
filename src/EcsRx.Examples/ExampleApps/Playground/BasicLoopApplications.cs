using System;
using System.Diagnostics;
using System.Numerics;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Lookups;
using EcsRx.Examples.Application;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Extensions;

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
    
    public class BasicLoopApplication : EcsRxConsoleApplication
    {
        private static readonly int EntityCount = 1000000;
        private IEntityCollection _collection;
        private IComponentTypeLookup _componentTypeLookup;
        
        protected override void ApplicationStarted()
        {
            _componentTypeLookup = Container.Resolve<IComponentTypeLookup>();
            _collection = EntityCollectionManager.GetCollection();
            
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponents(new BasicClassComponent());               
            }
           
            RunProcess();
        }

        private void RunProcess()
        {
            var componentId = _componentTypeLookup.GetComponentType(typeof(BasicClassComponent));
            var timer = Stopwatch.StartNew();
            foreach (var entity in _collection)
            {
                var basicComponent = (BasicClassComponent)entity.GetComponent(componentId);
                basicComponent.Position += Vector3.One;
                basicComponent.Something += 10;
            }
            timer.Stop();

            var totalTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Executed {EntityCount} entities in single thread in {totalTime}ms");
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