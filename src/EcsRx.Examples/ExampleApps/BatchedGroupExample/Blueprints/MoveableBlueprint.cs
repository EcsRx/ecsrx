using System;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Lookups;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample.Blueprints
{
    public class MoveableBlueprint : IBlueprint
    {
        private const float MinimumMovementSpeed = 1;
        private const float MaximumMovementSpeed = 5;
        
        private readonly Random _random = new Random();

        public void Apply(IEntity entity)
        {
            entity.AddComponent(new NameComponent {Name = $"BatchedEntity-{entity.Id}"});
            entity.AddComponent<PositionComponent>(ComponentLookupTypes.PositionComponentId);
                
            ref var movementSpeedComponent = ref entity.AddComponent<MovementSpeedComponent>(ComponentLookupTypes.MovementSpeedComponentId);
            movementSpeedComponent.Speed = (float)_random.NextDouble() * (MaximumMovementSpeed - MinimumMovementSpeed) + MinimumMovementSpeed;
        }
    }
}