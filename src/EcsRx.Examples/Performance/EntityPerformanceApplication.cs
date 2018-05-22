using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.Performance.Helper;
using EcsRx.Examples.Performance.Systems;
using EcsRx.Extensions;
using EcsRx.Systems;

namespace EcsRx.Examples.Performance
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
            _availableComponents = _groupFactory.GetComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            _entities = new List<IEntity>();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(Guid.NewGuid(), EventSystem);
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
            { _system.Execute(_entities[i]); }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
    }
}