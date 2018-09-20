using System;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.HealthExample.Blueprints;
using EcsRx.Examples.ExampleApps.HealthExample.Components;
using EcsRx.Examples.ExampleApps.HealthExample.Events;
using EcsRx.Examples.ExampleApps.HealthExample.Systems;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.HealthExample
{
    public class HealthExampleApplication : EcsRxConsoleApplication
    {
        private bool _quit;
        private IEntity _enemy;
        private readonly Random _random = new Random();

        protected override void ApplicationStarting()
        {
            this.RegisterSystem<TakeDamageSystem>();
            this.RegisterSystem<DisplayHealthChangesSystem>();
        }

        protected override void ApplicationStarted()
        {
            this.RegisterAllBoundSystems();
            
            var defaultPool = EntityCollectionManager.GetCollection();
            _enemy = defaultPool.CreateEntity(new EnemyBlueprint(100));

            HandleInput();
        }

        private void HandleInput()
        {
            var healthComponent = _enemy.GetComponent<HealthComponent>();

            while (!_quit)
            {
                var keyPressed = Console.ReadKey();
                if (keyPressed.Key == ConsoleKey.Spacebar)
                {
                    var eventArg = new EntityDamagedEvent(healthComponent, _random.Next(5, 25));
                    EventSystem.Publish(eventArg);
                }
                else if (keyPressed.Key == ConsoleKey.Escape)
                { _quit = true; }
            }
        }
    }
}