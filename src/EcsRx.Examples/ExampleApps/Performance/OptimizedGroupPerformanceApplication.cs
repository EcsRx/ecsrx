using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsRx.Components;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Examples.ExampleApps.Performance.Modules;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class OptimizedGroupPerformanceApplication : EcsRxConsoleApplication
    {
        private IComponent[] _availableComponents;
        private int[] _availableComponentTypeIds;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private readonly Random _random = new Random();

        protected override IDependencyModule GetFrameworkModule()
        { return new CustomFrameworkModule(); }

        protected override void ApplicationStarted()
        {
            _availableComponentTypeIds = Enumerable.Range(0, 20).ToArray();
            
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponents = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            var groups = _groupFactory.CreateTestGroups().ToArray();
            foreach (var group in groups)
            { EntityCollectionManager.GetObservableGroup(group); }

            var firstRun = ProcessEntities(10000);
            var secondRun = ProcessEntities(10000);
            var thirdRun = ProcessEntities(10000);

            Console.WriteLine($"Finished In: {(firstRun + secondRun + thirdRun).TotalSeconds}s");
            Console.WriteLine($"First Took: {firstRun.TotalSeconds}s");
            Console.WriteLine($"Second Took: {secondRun.TotalSeconds}s");
            Console.WriteLine($"Third Took: {thirdRun.TotalSeconds}s");
        }

        private TimeSpan ProcessEntities(int amount)
        {
            var defaultPool = EntityCollectionManager.GetCollection();
            EntityCollectionManager.Collections.ForEachRun(x => x.RemoveAllEntities());
            GC.Collect();
            
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < amount; i++)
            {
                var entity = defaultPool.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveComponents(_availableComponentTypeIds);
            }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
    }
}