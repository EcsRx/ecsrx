using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EcsRx.Collections.Entity;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Examples.ExampleApps.Performance.Systems;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class SimpleSystemApplication : EcsRxConsoleApplication
    {
        private static readonly int EntityCount = 1000;
        private IEntityCollection _collection;
        private ExampleReactToGroupSystem _groupSystem;

        protected override void ApplicationStarted()
        {
            _collection = EntityDatabase.GetCollection();
            _groupSystem = new ExampleReactToGroupSystem();
            
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = _collection.CreateEntity();
                entity.AddComponents(new SimpleReadComponent(), new SimpleWriteComponent());
            }

            RunSingleThread();
            RunMultiThreaded();
        }

        private void RunSingleThread()
        {
            var timer = Stopwatch.StartNew();
            foreach(var entity in _collection)
            { _groupSystem.Process(entity); }
            timer.Stop();

            var totalTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Executed {EntityCount} entities in single thread in {totalTime}ms");
        }
        
        private void RunMultiThreaded()
        {
            var timer = Stopwatch.StartNew();
            Parallel.ForEach(_collection, _groupSystem.Process);
            timer.Stop();

            var totalTime = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
            Console.WriteLine($"Executed {EntityCount} entities multi-threaded in {totalTime}ms");
        }
    }
}