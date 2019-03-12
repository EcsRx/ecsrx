using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Examples.ExampleApps.Performance.Modules;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class OptimizedEntityPerformanceApplication : EcsRxConsoleApplication
    {
        private IComponent[] _availableComponents;
        private int[] _availableComponentTypeIds;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private static readonly int EntityCount = 100000;

        private List<IEntity> _entities;

        protected override void LoadModules()
        { Container.LoadModule<OptimizedFrameworkModule>(); }
        
        protected override void BindSystems()
        {}

        protected override void ApplicationStarted()
        {                       
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            _availableComponentTypeIds = Enumerable.Range(0, 20).ToArray();
            
            var componentDatabase = Container.Resolve<IComponentDatabase>();
            var componentTypeLookup = Container.Resolve<IComponentTypeLookup>();
                        
            _entities = new List<IEntity>();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, componentDatabase, componentTypeLookup);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);                
            }

            var timeTaken = ProcessEntities();

            Console.WriteLine($"Finished In: {timeTaken.Milliseconds}ms");
        }

        private TimeSpan ProcessEntities()
        {
            EntityCollectionManager.Collections.ForEachRun(x => x.RemoveAllEntities());
            GC.Collect();
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < EntityCount; i++)
            { ProcessEntity(_entities[i]); }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
        
        public void ProcessEntity(IEntity entity)
        {
            // Called just to make sure the method runs
            bool ignore;
            
            if(entity.HasAllComponents(_availableComponentTypeIds))
            { ignore = true; }

            for (var i = 0; i < _availableComponentTypeIds.Length; i++)
            {
                var component = entity.GetComponent(i);
                if(component == null) { }
            }
        }
    }
}