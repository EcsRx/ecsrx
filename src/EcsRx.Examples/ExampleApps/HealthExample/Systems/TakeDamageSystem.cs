using SystemsRx.Systems.Conventional;
using EcsRx.Examples.ExampleApps.HealthExample.Events;

namespace EcsRx.Examples.ExampleApps.HealthExample.Systems
{
    public class TakeDamageSystem : IReactToEventSystem<EntityDamagedEvent>
    {
        public void Process(EntityDamagedEvent eventData)
        { eventData.HealthComponent.Health.Value -= eventData.DamageApplied; }
    }
}