using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Components.Lookups
{
    public class ComponentTypeLookup : IComponentTypeLookup
    {
        public IReadOnlyDictionary<Type, int> ComponentsByType { get; }
        public IReadOnlyDictionary<int, Type> ComponentsById { get; }

        public ComponentTypeLookup(IReadOnlyDictionary<Type, int> componentsByType)
        {
            ComponentsByType = componentsByType;
            ComponentsById = componentsByType.ToDictionary(x => x.Value, x => x.Key);
        }

        public int GetComponentType<T>() where T : IComponent
        { return GetComponentType(typeof(T)); }

        public int GetComponentType(Type type)
        { return ComponentsByType[type]; }
        
        public int[] GetComponentTypes(params Type[] types)
        { return types.Select(GetComponentType).ToArray(); }

        public Type[] GetComponentTypes(params int[] typeIds)
        { return typeIds.Select(x => ComponentsById[x]).ToArray(); }

        public IReadOnlyDictionary<Type, int> GetAllComponentTypes()
        { return ComponentsByType; }
    }
}