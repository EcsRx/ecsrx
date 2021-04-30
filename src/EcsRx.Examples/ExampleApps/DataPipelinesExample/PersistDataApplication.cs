using System;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Events;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Modules;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample
{
    public class PersistDataApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void LoadModules()
        {
            base.LoadModules();
            Container.LoadModule<PipelineModule>();
        }

        protected override void ApplicationStarted()
        {
            var defaultPool = EntityDatabase.GetCollection();
            var entity = defaultPool.CreateEntity();

            var component = new PlayerStateComponent
            {
                Name = "Super Player 1", 
                Level = 10, 
                SomeFieldThatWontBePersisted = "Wont Be Persisted"
            };
            entity.AddComponent(component);

            Console.WriteLine("This app posts your player state over HTTP which gets echoed back to you.");
            Console.WriteLine("This is a very useful thing if you use online apis like playfab etc");
            Console.WriteLine(" - Press Enter To Trigger Pipeline");
            Console.WriteLine(" - Press Escape To Quit");
            
            HandleInput();
        }

        private void HandleInput()
        {
            while (!_quit)
            {
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Enter)
                {
                    var eventArg = new SavePipelineEvent();
                    EventSystem.Publish(eventArg);
                }
                else if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }
        }
    }
}