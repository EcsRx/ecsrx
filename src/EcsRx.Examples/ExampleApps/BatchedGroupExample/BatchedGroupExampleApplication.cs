using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Blueprints;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Modules;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample
{
    public class BatchedGroupExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;
        private int _entityCount = 2;

        protected override void LoadModules()
        {
            base.LoadModules();
            Container.LoadModule<CustomComponentLookupsModule>();
        }

        protected override void ApplicationStarted()
        {
            var blueprint = new MoveableBlueprint();
            
            var defaultPool = EntityCollectionManager.EntityDatabase.GetCollection();

            for (var i = 0; i < _entityCount; i++)
            { defaultPool.CreateEntity(blueprint); }

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