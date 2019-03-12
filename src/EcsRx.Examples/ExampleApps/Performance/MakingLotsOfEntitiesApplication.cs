using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Examples.ExampleApps.Performance.Modules;
using EcsRx.Examples.ExampleApps.Performance.Systems;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class MakingLotsOfEntitiesApplication : EcsRxConsoleApplication
    {
        private static readonly int EntityCount = 10000;

        protected override void BindSystems()
        {
            Container.Bind<ISystem, ExampleBatchedGroupSystem>();
        }

        protected override void ApplicationStarted()
        {
            var collection = EntityCollectionManager.GetCollection();
            
           
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