using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Components.Lookups
{
    public class ComponentTypeLookup : IComponentTypeLookup
    {
        public IReadOnlyDictionary<Type, int> ComponentsByType { get; }
        public IReadOnlyDictionary<int, Type> ComponentsById { get; }
        public bool[] ComponentStructLookups { get; }
        public int[] AllComponentTypeIds { get; }

        public ComponentTypeLookup(IReadOnlyDictionary<Type, int> componentsByType)
        {
            ComponentsByType = componentsByType;
            ComponentsById = componentsByType.ToDictionary(x => x.Value, x => x.Key);
            ComponentStructLookups = componentsByType.Keys.Select(x => x.IsValueType).ToArray();
            AllComponentTypeIds = componentsByType.Values.ToArray();
        }

        public int GetComponentType<T>() where T : IComponent
        { return GetComponentType(typeof(T)); }

        public int GetComponentType(Type type)
        { return ComponentsByType[type]; }
        
        public int[] GetComponentTypes(params Type[] types)
        { return types.Select(GetComponentType).ToArray(); }

        public Type[] GetComponentTypes(params int[] typeIds)
        { return typeIds.Select(x => ComponentsById[x]).ToArray(); }

        public bool IsComponentStruct(int componentTypeId)
        { return ComponentStructLookups[componentTypeId]; }

        public T CreateDefault<T>() where T : IComponent, new()
        { return new T(); }

        public IReadOnlyDictionary<Type, int> GetAllComponentTypes()
        { return ComponentsByType; }
    }
}