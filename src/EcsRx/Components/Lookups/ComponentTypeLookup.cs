using System;
using System.Collections.Generic;

namespace EcsRx.Components
{
    public class ComponentTypeLookup : IComponentTypeLookup
    {
        public IDictionary<Type, int> ComponentTypes { get; }
        
        public int GetComponentType<T>()
        { return ComponentTypes[typeof(T)]; }

        public int GetComponentType(Type type)
        { return ComponentTypes[type]; }
    }
}