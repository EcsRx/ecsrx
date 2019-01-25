using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.Custom.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.Custom
{
    public class SetupSystemPriorityApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarted()
        {
            var defaultPool = EntityCollectionManager.GetCollection();
            var entity = defaultPool.CreateEntity();
            
            entity.AddComponents(new FirstComponent());

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