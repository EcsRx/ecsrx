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
        public bool[] ComponentDisposableLookups { get; }
        public int[] AllComponentTypeIds { get; }

        public ComponentTypeLookup(IReadOnlyDictionary<Type, int> componentsByType)
        {
            ComponentsByType = componentsByType;
            ComponentsById = componentsByType.ToDictionary(x => x.Value, x => x.Key);
            AllComponentTypeIds = componentsByType.Values.ToArray();
            
            ComponentStructLookups = componentsByType.Keys
                .Select(x => x.IsValueType)
                .ToArray();

            ComponentDisposableLookups = componentsByType.Keys
                .Select(x => x.GetInterfaces().Any(y => y == typeof(IDisposable)))
                .ToArray();
        }

        public int GetComponentTypeId(Type type)
        { return ComponentsByType[type]; }

        public Type GetComponentType(int typeId)
        { return ComponentsById[typeId]; }

        public bool IsComponentStruct(int componentTypeId)
        { return ComponentStructLookups[componentTypeId]; }

        public bool IsComponentDisposable(int componentTypeId)
        { return ComponentDisposableLookups[componentTypeId]; }

        public IReadOnlyDictionary<Type, int> GetComponentTypeMappings()
        { return ComponentsByType; }
    }
}