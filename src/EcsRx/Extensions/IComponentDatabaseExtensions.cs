using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Components.Database;

namespace EcsRx.Extensions
{
    public static class IComponentDatabaseExtensions
    {
        public static IComponent Get(this IComponentDatabase componentDatabase, int entityId, int componentTypeId)
        { return componentDatabase.Get<IComponent>(componentTypeId, entityId); }

        public static IReadOnlyList<IComponent> GetComponents(this IComponentDatabase componentDatabase, int componentTypeId)
        { return componentDatabase.GetComponents<IComponent>(componentTypeId); }
    }
}