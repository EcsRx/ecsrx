using System;
using BenchmarkDotNet.Running;

namespace EcsRx.PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GroupPerformanceScenario>();
            BenchmarkRunner.Run<EntityUpdatesScenario>();
        }
    }
}
