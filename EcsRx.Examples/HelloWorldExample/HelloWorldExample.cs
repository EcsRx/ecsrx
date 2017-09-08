using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.HelloWorldExample.Components;
using EcsRx.Examples.HelloWorldExample.Systems;

namespace EcsRx.Examples.HelloWorldExample
{
    public class HelloWorldExampleApplication : EcsRxApplication
    {
        private bool _quit = false;

        public override void StartApplication()
        {
            SystemExecutor.AddSystem(new TalkingSystem());

            var defaultPool = PoolManager.GetPool();
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