using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Components.Lookups
{
    public class DefaultComponentTypeAssigner : IComponentTypeAssigner
    {
        public IEnumerable<Type> GetAllComponentTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var componentType = typeof(IComponent);

            return assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => componentType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);
        }
        
        public IReadOnlyDictionary<Type, int> GenerateComponentLookups()
        {
            var lookupId = 0;
            return GetAllComponentTypes()?.ToDictionary(x => x, _ => lookupId++);
        }
    }
}