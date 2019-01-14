using System;
using EcsRx.Examples.ExampleApps.ComputedGroupExample;
using EcsRx.Examples.ExampleApps.Performance;
using EcsRx.Examples.ExampleApps.Playground.ClassBased;
using EcsRx.Examples.ExampleApps.Playground.StructBased;

namespace EcsRx.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            //var application = new SimpleSystemApplication();
            //var application = new GroupPerformanceApplication();
            //var application = new OptimizedGroupPerformanceApplication();
            //var application = new OptimizedEntityPerformanceApplication();
            //var application = new EntityPerformanceApplication();
            //var application = new ComputedGroupExampleApplication();
            //var application = new HealthExampleApplication();
            //var application = new HelloWorldExampleApplication();   // Comment out this and uncomment the other one to run the other example

            //application.StartApplication();
           
            // Playground examples
            /*
            new Class1Application().StartApplication();
            new Struct1Application().StartApplication();
            new Class2Application().StartApplication();
            new Struct2Application().StartApplication();
            new Class3Application().StartApplication();
            new Struct3Application().StartApplication();
            new Class4Application().StartApplication();
            new Struct4Application().StartApplication();
            new Struct5Application().StartApplication();
            */
            
            //new Struct1Application().StartApplication();
            new Struct4Application().StartApplication();
        }
    }
}
