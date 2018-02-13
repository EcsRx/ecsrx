using EcsRx.Examples.HealthExample.Components;

namespace EcsRx.Examples.HealthExample.Events
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