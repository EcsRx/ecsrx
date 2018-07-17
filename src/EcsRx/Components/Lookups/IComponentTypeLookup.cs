using System;
using System.Collections.Generic;

namespace EcsRx.Components
{
    public interface IComponentTypeLookup
    {
        IReadOnlyDictionary<Type, int> GetAllComponentTypes();
        int GetComponentType<T>() where T : IComponent;
        int GetComponentType(Type type);
        int[] GetComponentTypes(params Type[] types);
    }
}