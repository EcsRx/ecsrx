using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.HelloWorldExample.Components;
using EcsRx.Examples.HelloWorldExample.Systems;

namespace EcsRx.Examples.HelloWorldExample
{
    public class HelloWorldExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarting()
        {
            RegisterSystem<TalkingSystem>();
        }

        protected override void ApplicationStarted()
        {
            RegisterAllBoundSystems();

            var defaultPool = EntityCollectionManager.GetCollection();
            var entity = defaultPool.CreateEntity();

            var canTalkComponent = new CanTalkComponent {Message = "Hello world"};
            entity.AddComponent(canTalkComponent);

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