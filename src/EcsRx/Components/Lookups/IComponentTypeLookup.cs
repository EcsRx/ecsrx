using System;
using System.Collections.Generic;

namespace EcsRx.Components.Lookups
{
    /// <summary>
    /// The Component Type Lookup interface is responsible for looking up
    /// component ids for the types as well as vice versa. 
    /// </summary>
    public interface IComponentTypeLookup
    {
        int[] AllComponentTypeIds { get; }
        IReadOnlyDictionary<Type, int> GetComponentTypeMappings();
        int GetComponentTypeId(Type type);
        Type GetComponentType(int typeId);
        bool IsComponentStruct(int componentTypeId);
        bool IsComponentDisposable(int componentTypeId);
    }
}