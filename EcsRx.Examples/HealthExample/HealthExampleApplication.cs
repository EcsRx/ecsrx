using System;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.HealthExample.Blueprints;
using EcsRx.Examples.HealthExample.Components;
using EcsRx.Examples.HealthExample.Events;
using EcsRx.Examples.HealthExample.Systems;

namespace EcsRx.Examples.HealthExample
{
    public class HealthExampleApplication : EcsRxApplication
    {
        private bool _quit = false;
        private IEntity _enemy;
        private readonly Random _random = new Random();

        public override void StartApplication()
        {
            SystemExecutor.AddSystem(new TakeDamageSystem(EventSystem));
            SystemExecutor.AddSystem(new DisplayHealthChangesSystem());

            var defaultPool = PoolManager.GetPool();
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