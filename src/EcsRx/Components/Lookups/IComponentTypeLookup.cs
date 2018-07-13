using System;

namespace EcsRx.Components
{
    public interface IComponentTypeLookup
    {
        int GetComponentType<T>();
        int GetComponentType(Type type);
    }
}