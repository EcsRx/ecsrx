using System;
using System.Collections.Generic;

namespace EcsRx.Components.Lookups
{
    public interface IComponentTypeLookup
    {
        IReadOnlyDictionary<Type, int> GetAllComponentTypes();
        int GetComponentType<T>() where T : IComponent;
        int GetComponentType(Type type);
        int[] GetComponentTypes(params Type[] types);
        Type[] GetComponentTypes(params int[] typeIds);
        bool IsComponentStruct(int componentTypeId);
        int TotalComponentTypes();
        T CreateDefault<T>() where T : IComponent, new();
    }
}