using EcsRx.Events;
using EcsRx.Examples.HealthExample.Events;
using EcsRx.Systems.Custom;

namespace EcsRx.Examples.HealthExample.Systems
{
    public class TakeDamageSystem : EventReactionSystem<EntityDamagedEvent>
    {
        public TakeDamageSystem(IEventSystem eventSystem) : base(eventSystem)
        {}

        public override void EventTriggered(EntityDamagedEvent eventData)
        { eventData.HealthComponent.Health.Value -= eventData.DamageApplied; }
    }
}