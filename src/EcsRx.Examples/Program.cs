using System;
using EcsRx.Examples.ExampleApps.ComputedGroupExample;
using EcsRx.Examples.ExampleApps.Performance;

namespace EcsRx.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            //var application = new SimpleSystemApplication();
            //var application = new GroupPerformanceApplication();
            var application = new OptimizedGroupPerformanceApplication();
            //var application = new OptimizedEntityPerformanceApplication();
            //var application = new EntityPerformanceApplication();
            //var application = new ComputedGroupExampleApplication();
            //var application = new HealthExampleApplication();
            //var application = new HelloWorldExampleApplication();   // Comment out this and uncomment the other one to run the other example

            //var application = new SimplestForEachLoopApplication();
            //var application = new BasicForEachLoopApplication();
            //var application = new BasicForLoopApplication();
            //var application = new BasicStructForLoopApplication();
            //var application = new BatchedForLoopApplication();
            //var application = new BatchedStructForLoopApplication();
            //var application = new MultithreadedBatchedStructForLoopApplication();
            
            application.StartApplication();
        }
    }
}
