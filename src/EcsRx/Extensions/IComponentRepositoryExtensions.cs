using System;
using EcsRx.Components;
using EcsRx.Components.Database;

namespace EcsRx.Extensions
{
    public static class IComponentRepositoryExtensions
    {
        public static IComponent Get(this IComponentRepository componentRepository, int entityId, Type componentType)
        {
            var componentTypeId = componentRepository.ComponentTypeLookup.GetComponentType(componentType);
            return componentRepository.Get(entityId, componentTypeId);
        }

        public static bool Has(this IComponentRepository componentRepository, int entityId, Type componentType)
        {
            var componentTypeId = componentRepository.ComponentTypeLookup.GetComponentType(componentType);
            return componentRepository.Has(entityId, componentTypeId);
        }

        public static void Remove(this IComponentRepository componentRepository, int entityId, Type componentType)
        {
            var componentTypeId = componentRepository.ComponentTypeLookup.GetComponentType(componentType);
            componentRepository.Remove(entityId, componentTypeId);
        }

        public static IComponent Get(this IComponentRepository componentRepository, int entityId, int componentTypeId)
        { return componentRepository.Get<IComponent>(entityId, componentTypeId); }
    }
}