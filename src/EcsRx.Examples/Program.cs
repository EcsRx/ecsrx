using System;
using EcsRx.Examples.Custom;
using EcsRx.Examples.ExampleApps.BatchedGroupExample;
using EcsRx.Examples.ExampleApps.ComputedGroupExample;
using EcsRx.Examples.ExampleApps.DataPipelinesExample;
using EcsRx.Examples.ExampleApps.HealthExample;
using EcsRx.Examples.ExampleApps.HelloWorldExample;
using EcsRx.Examples.ExampleApps.LoadingEntityDatabase;
using EcsRx.Examples.ExampleApps.Performance;
using EcsRx.Examples.ExampleApps.Playground.ClassBased;
using EcsRx.Examples.ExampleApps.Playground.StructBased;
using Spectre.Console;

namespace EcsRx.Examples
{
    class Program
    {
        class Example
        {
            public string Name { get; }
            public Action Executor { get; }

            public Example(string name, Action executor)
            {
                Name = name;
                Executor = executor;
            }
        }
        
        static void Main(string[] args)
        {
            new ObservableGroupPerformanceApplication().StartApplication();
            return;
        /*
            var availableExamples = new []
            {
                new Example("Scenario: Hello World", () => new HelloWorldExampleApplication().StartApplication()),
                new Example("Scenario: Computed Groups", () => new ComputedGroupExampleApplication().StartApplication()),
                new Example("Scenario: Health Deduction", () => new HealthExampleApplication().StartApplication()),
                new Example("Scenario: Data Persistence", () => new PersistDataApplication().StartApplication()),
                new Example("Scenario: Entity Databases", () => new LoadingEntityDatabaseApplication().StartApplication()),
                new Example("Scenario: Batched Groups", () => new BatchedGroupExampleApplication().StartApplication()),
                new Example("Scenario: System Priorities", () => new SetupSystemPriorityApplication().StartApplication()),
                
                new Example("Performance: Systems", () => new SimpleSystemApplication().StartApplication()),
                new Example("Performance: Default Entity", () => new EntityPerformanceApplication().StartApplication()),
                new Example("Performance: Optimised Entity", () => new OptimizedEntityPerformanceApplication().StartApplication()),
                new Example("Performance: Default Group", () => new GroupPerformanceApplication().StartApplication()),
                new Example("Performance: Optimised Group", () => new OptimizedGroupPerformanceApplication().StartApplication()),
                new Example("Performance: Entity Creation", () => new MakingLotsOfEntitiesApplication().StartApplication()),
                
                new Example("Dev: Using Classes 1", () => new Class1Application().StartApplication()),
                new Example("Dev: Using Classes 2", () => new Class2Application().StartApplication()),
                new Example("Dev: Using Classes 3", () => new Class3Application().StartApplication()),
                new Example("Dev: Using Classes 4", () => new Class4Application().StartApplication()),
                new Example("Dev: Using Structs 1", () => new Struct1Application().StartApplication()),
                new Example("Dev: Using Structs 2", () => new Struct2Application().StartApplication()),
                new Example("Dev: Using Structs 3", () => new Struct3Application().StartApplication()),
                new Example("Dev: Using Structs 4", () => new Struct4Application().StartApplication()),
                new Example("Dev: Using Structs 4", () => new Struct4BApplication().StartApplication())
            };

            var exampleSelector = new SelectionPrompt<Example>()
                    .Title("Welcome To The [green]EcsRx[/] examples, what example do you want to run?")
                    .PageSize(50)
                    .MoreChoicesText("[grey](Move up and down to reveal more examples)[/]")
                    .UseConverter(x => x.Name)
                    .AddChoices(availableExamples);
            
            var exampleToRun = AnsiConsole.Prompt(exampleSelector);
            exampleToRun.Executor();*/
        }
    }
}
