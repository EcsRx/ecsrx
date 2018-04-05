using System;
using System.Linq;
using EcsRx.Components;
using EcsRx.Examples.Application;
using EcsRx.Examples.GroupPerformance.Helper;
using EcsRx.Extensions;

namespace EcsRx.Examples.GroupPerformance
{
    public class GroupPerformanceApplication : EcsRxConsoleApplication
    {
        private IComponent[] _availableComponents;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private readonly Random _random = new Random();

        protected override void ApplicationStarted()
        {
            _availableComponents = _groupFactory.GetComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            var groups = _groupFactory.CreateTestGroups().ToArray();
            foreach (var group in groups)
            { EntityCollectionManager.CreateObservableGroup(group); }

            var firstRun = ProcesEntities(10000);
            var secondRun = ProcesEntities(10000);
            var thirdRun = ProcesEntities(10000);

            Console.WriteLine($"Finished In: {(firstRun + secondRun + thirdRun).TotalSeconds}s");
            Console.WriteLine($"First Took: {firstRun.TotalSeconds}s");
            Console.WriteLine($"Second Took: {secondRun.TotalSeconds}s");
            Console.WriteLine($"Third Took: {thirdRun.TotalSeconds}s");
            Console.WriteLine("Press Enter To Exit");
            Console.ReadKey();
        }

        private TimeSpan ProcesEntities(int amount)
        {
            var defaultPool = EntityCollectionManager.GetCollection();
            EntityCollectionManager.Pools.ForEachRun(x => x.RemoveAllEntities());
            var startTime = DateTime.Now;

            for (var i = 0; i < amount; i++)
            {
                var entity = defaultPool.CreateEntity();
                entity.AddComponents(_availableComponents);
                entity.RemoveComponents(_availableComponents);
            }
            
            var endTime = DateTime.Now;
            return endTime - startTime;
        }
    }
}