using EcsRx.Blueprints;
using EcsRx.Entities;

namespace EcsRx.Extensions
{
    public static class IBlueprintExtensions
    {
        public static void ApplyToAll(this IBlueprint blueprint, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { blueprint.Apply(entities[i]); }
        }
    }
}