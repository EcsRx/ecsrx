using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EcsRx.Components;
using EcsRx.Components.Accessor;
using EcsRx.Components.Lookups;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class IComponentTypeLookupExtensions
    {
        public static int GetComponentType<T>(this IComponentTypeLookup typeLookup) where T : IComponent
        { return typeLookup.GetComponentTypeId(typeof(T)); }
        
        public static T CreateDefault<T>(this IComponentTypeLookup typeLookup) where T : IComponent, new()
        { return new T(); }

        public static IComponent CreateDefault(this IComponentTypeLookup typeLookup, int typeId)
        { return Activator.CreateInstance(typeLookup.GetComponentType(typeId)) as IComponent; }
        
        public static int[] GetComponentTypeIds(this IComponentTypeLookup typeLookup, params Type[] types)
        { return types.Select(typeLookup.GetComponentTypeId).ToArray(); }

        public static Type[] GetComponentTypes(this IComponentTypeLookup typeLookup, params int[] typeIds)
        { return typeIds.Select(typeLookup.GetComponentType).ToArray(); }
        
        public static int[] GetComponentTypes(this IComponentTypeLookup typeLookup, IEnumerable<Type> types)
        { return types.Select(typeLookup.GetComponentTypeId).ToArray(); }

        public static Type[] GetComponentTypes(this IComponentTypeLookup typeLookup, IEnumerable<int> typeIds)
        { return typeIds.Select(typeLookup.GetComponentType).ToArray(); }

        public static IComponentAccessor<T> GetAccessorFor<T>(this IComponentTypeLookup typeLookup)
        {
            var componentTypeId = typeLookup.GetComponentTypeId(typeof(T));
            return new ComponentAccessor<T>(componentTypeId);
        }

        public static LookupGroup GetLookupGroupFor(this IComponentTypeLookup typeLookup, IGroup group)
        {
            var requiredComponents = typeLookup.GetComponentTypeIds(group.RequiredComponents);
            var excludedComponents = typeLookup.GetComponentTypeIds(group.ExcludedComponents);
            return new LookupGroup(requiredComponents, excludedComponents);
        }
    }
}