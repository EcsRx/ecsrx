using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsRx.Extensions;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Extensions
{
    public static class IEntityExtensions
    {
        public static int GetHealthPercentile(this IEntity entity)
        {
            var healthComponent = entity.GetComponent<HasHealthComponent>();
            var percentage = ((float)healthComponent.CurrentHealth / (float)healthComponent.MaxHealth) * 100;
            return (int) percentage;
        }
        
        public static string GetHealthString(this IEntity entity)
        {
            var healthComponent = entity.GetComponent<HasHealthComponent>();
            return $"{healthComponent.CurrentHealth}/{healthComponent.MaxHealth}";
        }
        
        public static string GetName(this IEntity entity)
        {
            var nameComponent = entity.GetComponent<HasNameComponent>();
            return nameComponent.Name;
        }
    }
}