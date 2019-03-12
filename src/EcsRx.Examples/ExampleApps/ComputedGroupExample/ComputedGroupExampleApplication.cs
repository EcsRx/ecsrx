using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Blueprints;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Modules;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample
{
    public class ComputedGroupExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void LoadModules()
        {
            base.LoadModules();
            Container.LoadModule<ComputedModule>();
        }

        protected override void ApplicationStarted()
        {
            var defaultPool = EntityCollectionManager.GetCollection();
            defaultPool.CreateEntity(new CharacterBlueprint("Bob", 200));
            defaultPool.CreateEntity(new CharacterBlueprint("Tom", 150));
            defaultPool.CreateEntity(new CharacterBlueprint("Rolf", 150));
            defaultPool.CreateEntity(new CharacterBlueprint("Mez", 100));
            defaultPool.CreateEntity(new CharacterBlueprint("TP", 1000));
            defaultPool.CreateEntity(new CharacterBlueprint("MasterChief", 100));
            defaultPool.CreateEntity(new CharacterBlueprint("Weakling", 20));

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