using EcsRx.Examples.ExampleApps.HealthExample.Components;

namespace EcsRx.Examples.ExampleApps.HealthExample.Events
{
    public class EntityDamagedEvent
    {
        public HealthComponent HealthComponent { get; }
        public float DamageApplied { get; }

        public EntityDamagedEvent(HealthComponent healthComponent, float damageApplied)
        {
            HealthComponent = healthComponent;
            DamageApplied = damageApplied;
        }
    }
}