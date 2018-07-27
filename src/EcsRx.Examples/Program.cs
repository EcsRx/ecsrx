using System;
using EcsRx.Examples.ExampleApps.ComputedGroupExample;
using EcsRx.Examples.ExampleApps.Performance;

namespace EcsRx.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new GroupPerformanceApplication();
            //var application = new OptimizedGroupPerformanceApplication();();
            //var application = new OptimizedEntityPerformanceApplication();
            //var application = new EntityPerformanceApplication();
            //var application = new ComputedGroupExampleApplication();
            //var application = new HealthExampleApplication();
            //var application = new HelloWorldExampleApplication();   // Comment out this and uncomment the other one to run the other example

            application.StartApplication();
        }
    }
}
