using System;
using System.Reactive.Linq;
using System.Text;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.HealthExample.Components;
using EcsRx.Examples.Extensions;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;

namespace EcsRx.Examples.ExampleApps.HealthExample.Systems
{
    public class DisplayHealthChangesSystem : IReactToDataSystem<float>
    {
        public IGroup Group => new Group(typeof(HealthComponent));
        private const int HealthSegments = 10;

        public IObservable<float> ReactToData(IEntity entity)
        {
            var healthComponent = entity.GetComponent<HealthComponent>();
            return healthComponent.Health.WithValueChange().Select(CalculateDamageTaken);
        }

        public void Process(IEntity entity, float damageDone)
        {
            var healthComponent = entity.GetComponent<HealthComponent>();

            Console.Clear();
            DisplayHealth(healthComponent, damageDone);

            if (healthComponent.Health.Value > 0)
            { Console.WriteLine("Press Space To Attack"); }
            else
            {
                Console.WriteLine("Enemy Is Dead! Hooray etc");
                Console.WriteLine(" - Press Escape To Quit -");
                entity.RemoveComponents(healthComponent);
            }
        }

        private static float CalculateDamageTaken(ValueChanges<float> values)
        {
            if (values.PreviousValue == 0) { return 0; }
            return values.PreviousValue - values.CurrentValue;
        }

        private static void DisplayHealth(HealthComponent healthComponent, float damageDone)
        {
            var healthPercentage = (healthComponent.Health.Value / healthComponent.MaxHealth) * 100;
            var healthSegments = (int)(healthPercentage / HealthSegments);

            if (healthSegments == 0 && healthPercentage > 0)
            { healthSegments = 1; }

            var healthText = new StringBuilder();
            for (var i = 0; i < HealthSegments; i++)
            {
                var hasSegment = i < healthSegments;
                healthText.Append(hasSegment ? "=" : " ");
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Health");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{healthText}]");
            Console.ResetColor();
            Console.WriteLine();

            if (damageDone >= 1)
            {
                Console.WriteLine($"You did {(int)damageDone} damage to the enemy");
                Console.WriteLine();
            }
        }
    }
}