using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Components
{
    public class ComponentTypeLookup : IComponentTypeLookup
    {
        public IReadOnlyDictionary<Type, int> ComponentTypes { get; }

        public ComponentTypeLookup(IReadOnlyDictionary<Type, int> componentTypes)
        {
            ComponentTypes = componentTypes;
        }

        public int GetComponentType<T>() where T : IComponent
        { return GetComponentType(typeof(T)); }

        public int GetComponentType(Type type)
        { return ComponentTypes[type]; }
        
        public int[] GetComponentTypes(params Type[] types)
        { return types.Select(GetComponentType).ToArray(); }

        public IReadOnlyDictionary<Type, int> GetAllComponentTypes()
        { return ComponentTypes; }
    }
}