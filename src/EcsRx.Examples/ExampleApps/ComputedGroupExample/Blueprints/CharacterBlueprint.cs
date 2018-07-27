using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Blueprints
{
    public class CharacterBlueprint : IBlueprint
    {
        public string Name { get; }
        public int Health { get; }

        public CharacterBlueprint(string name, int health)
        {
            Name = name;
            Health = health;
        }

        public void Apply(IEntity entity)
        {
            var healthComponent = new HasHealthComponent
            {
                CurrentHealth = Health,
                MaxHealth = Health
            };

            var nameComponent = new HasNameComponent
            {
                Name = Name
            };

            entity.AddComponents(nameComponent, healthComponent);
        }
    }
}