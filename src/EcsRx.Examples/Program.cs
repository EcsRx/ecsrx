using System;
using EcsRx.Examples.ExampleApps.BatchedGroupExample;
using EcsRx.Examples.ExampleApps.ComputedGroupExample;
using EcsRx.Examples.ExampleApps.HealthExample;
using EcsRx.Examples.ExampleApps.HelloWorldExample;
using EcsRx.Examples.ExampleApps.Performance;
using EcsRx.Examples.ExampleApps.Playground.ClassBased;
using EcsRx.Examples.ExampleApps.Playground.StructBased;

namespace EcsRx.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sample examples
            //new HelloWorldExampleApplication().StartApplication();
            //new ComputedGroupExampleApplication().StartApplication();
            //new HealthExampleApplication().StartApplication();
            new BatchedGroupExampleApplication().StartApplication();
            
            // Performance examples
            //new SimpleSystemApplication().StartApplication();
            //new GroupPerformanceApplication().StartApplication();
            //new OptimizedGroupPerformanceApplication().StartApplication();
            //new OptimizedEntityPerformanceApplication().StartApplication();
            //new EntityPerformanceApplication().StartApplication();
           
            // Playground examples
            //new Class1Application().StartApplication();
            //new Struct1Application().StartApplication();
            //new Class2Application().StartApplication();
            //new Struct2Application().StartApplication();
            //new Class3Application().StartApplication();
            //new Struct3Application().StartApplication();
            //new Class4Application().StartApplication();
            //new Struct4Application().StartApplication();            
            //new Struct4BApplication().StartApplication();
        }
    }
}
