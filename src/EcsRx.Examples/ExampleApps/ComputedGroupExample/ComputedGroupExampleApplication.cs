using System;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Blueprints;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.ComputedGroups;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Systems;
using EcsRx.Groups;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample
{
    public class ComputedGroupExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;

        protected override void ApplicationStarting()
        {
            RegisterSystem<RandomlyChangeHpSystem>();
            base.ApplicationStarting();
        }

        protected override void ApplicationStarted()
        {
            var namedHealthGroup = EntityCollectionManager.GetObservableGroup(new Group(typeof(HasHealthComponent), typeof(HasNameComponent)));
            var computedGroup = new LowestHealthComputedGroup(namedHealthGroup);
            var displayHealthSystem = new DisplayLowestHealthSystem(computedGroup);
                
            SystemExecutor.AddSystem(displayHealthSystem);
            
            RegisterAllBoundSystems();

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