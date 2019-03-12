using EcsRx.Components;
using EcsRx.Components.Database;

namespace EcsRx.Extensions
{
    public static class IComponentDatabaseExtensions
    {
        public static IComponent Get(this IComponentDatabase componentDatabase, int allocationIndex, int componentTypeId)
        { return componentDatabase.Get<IComponent>(componentTypeId, allocationIndex); }
    }
}