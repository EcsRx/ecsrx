using System;
using EcsRx.Examples.GroupPerformance;
using EcsRx.Examples.HealthExample;
using EcsRx.Examples.HelloWorldExample;

namespace EcsRx.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new GroupPerformanceApplication();
            //var application = new HealthExampleApplication();
            //var application = new HelloWorldExampleApplication();   // Comment out this and uncomment the other one to run the other example

            application.StartApplication();
        }
    }
}
