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

        public ComponentTypeLookup(IReadOnlyDictionary<Type, int> componentsByType)
        {
            ComponentsByType = componentsByType;
            ComponentsById = componentsByType.ToDictionary(x => x.Value, x => x.Key);
            ComponentStructLookups = componentsByType.Keys.Select(x => x.IsValueType).ToArray();
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

        public int TotalComponentTypes()
        { return ComponentsByType.Count; }

        public T CreateDefault<T>() where T : IComponent, new()
        {
            var type = typeof(T);
            if (type.IsValueType)
            { return default(T); }

            return Activator.CreateInstance<T>();
        }

        public IReadOnlyDictionary<Type, int> GetAllComponentTypes()
        { return ComponentsByType; }
    }
}