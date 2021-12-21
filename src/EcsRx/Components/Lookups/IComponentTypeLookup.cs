using System;
using System.Collections.Generic;

namespace EcsRx.Components.Lookups
{
    public interface IComponentTypeLookup
    {
        int[] AllComponentTypeIds { get; }

        IReadOnlyDictionary<Type, int> GetAllComponentTypes();
        int GetComponentType<T>() where T : IComponent;
        int GetComponentType(Type type);
        int[] GetComponentTypes(params Type[] types);
        Type[] GetComponentTypes(params int[] typeIds);
        bool IsComponentStruct(int componentTypeId);
        bool IsComponentDisposable(int componentTypeId);
        T CreateDefault<T>() where T : IComponent, new();
    }
}