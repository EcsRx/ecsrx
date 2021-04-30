using System;
using System.Diagnostics;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Systems;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Examples.ExampleApps.Performance.Systems;
using EcsRx.Extensions;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class MakingLotsOfEntitiesApplication : EcsRxConsoleApplication
    {
        private static readonly int EntityCount = 100000;

        protected override void BindSystems()
        {
            Container.Bind<ISystem, ExampleBatchedSystem>();
        }

        protected override void ApplicationStarted()
        {
            var collection = EntityDatabase.GetCollection();
            
           
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = collection.CreateEntity();
                entity.AddComponents(new SimpleReadComponent(), new SimpleWriteComponent());
            }
            stopwatch.Stop();
            Console.WriteLine($"Finished In: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}