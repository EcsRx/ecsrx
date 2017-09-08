using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Examples.HealthExample.Components;
using EcsRx.Reactive;

namespace EcsRx.Examples.HealthExample.Blueprints
{
    public class EnemyBlueprint : IBlueprint
    {
        public float Health { get; }

        public EnemyBlueprint(float health)
        {
            Health = health;
        }

        public void Apply(IEntity entity)
        {
            var healthComponent = new HealthComponent
            {
                Health = new ReactiveProperty<float>(Health),
                MaxHealth = Health
            };
            entity.AddComponent(healthComponent);
        }
    }
}