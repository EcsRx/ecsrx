using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Components
{
    public class DefaultComponentTypeAssigner : IComponentTypeAssigner
    {
        public IEnumerable<Type> GetAllComponentTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var componentType = typeof(IComponent);

            return assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => componentType.IsAssignableFrom(p));
        }
        
        public IDictionary<Type, int> GenerateComponentLookups()
        {
            var lookupId = 0;
            var componentTypes = GetAllComponentTypes();
            return componentTypes.ToDictionary(x => x, x => lookupId++);
        }
    }
}