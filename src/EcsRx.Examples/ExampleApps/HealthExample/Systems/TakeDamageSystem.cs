using SystemsRx.Events;
using EcsRx.Events;
using EcsRx.Examples.ExampleApps.HealthExample.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;

namespace EcsRx.Examples.ExampleApps.HealthExample.Systems
{
    public class TakeDamageSystem : EventReactionSystem<EntityDamagedEvent>
    {
        public TakeDamageSystem(IEventSystem eventSystem) : base(eventSystem)
        {}

        public override void EventTriggered(EntityDamagedEvent eventData)
        { eventData.HealthComponent.Health.Value -= eventData.DamageApplied; }
    }
}