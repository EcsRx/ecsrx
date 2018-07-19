using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Examples.ExampleApps.Performance.Systems;
using EcsRx.Extensions;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class EntityPerformanceApplication : EcsRxConsoleApplication
    {
        private IComponent[] _availableComponents;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private static readonly int EntityCount = 100000;

        private IReactToEntitySystem _system;
        private List<IEntity> _entities;

        protected override void ApplicationStarted()
        {
            var componentTypeAssigner = new DefaultComponentTypeAssigner();
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            var componentLookup = new ComponentTypeLookup(allComponents);
            
            _availableComponents = allComponents.Keys
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            var componentDatabase = new ComponentDatabase(componentLookup);
            var componentRepository = new ComponentRepository(componentLookup, componentDatabase);
            
            _entities = new List<IEntity>();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, componentRepository);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);
            }

            _system = new EntityUpdateSystem();

            var timeTaken = ProcessEntities();

            Console.WriteLine($"Finished In: {timeTaken.Milliseconds}ms");
            Console.WriteLine("Press Enter To Exit");
            Console.ReadKey();
        }

        private TimeSpan ProcessEntities()
        {
            EntityCollectionManager.Collections.ForEachRun(x => x.RemoveAllEntities());
            GC.Collect();
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < EntityCount; i++)
            { _system.Process(_entities[i]); }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
    }
}