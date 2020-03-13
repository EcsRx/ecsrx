using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.HelloWorldExample.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.HelloWorldExample
{
    public class HelloWorldExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarted()
        {
            var defaultPool = EntityCollectionManager.EntityDatabase.GetCollection();
            var entity = defaultPool.CreateEntity();

            var canTalkComponent = new CanTalkComponent {Message = "Hello world"};
            entity.AddComponents(canTalkComponent);

            HandleInput();
        }

        private void HandleInput()
        {
            while (!_quit)
            {
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }
        }
    }
}