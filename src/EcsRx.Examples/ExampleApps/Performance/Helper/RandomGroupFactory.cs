using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Groups;

namespace EcsRx.Examples.ExampleApps.Performance.Helper
{
    public class RandomGroupFactory
    {
        public IEnumerable<Type> GetComponentTypes => _componentTypes;
        private List<Type> _componentTypes;

        public RandomGroupFactory()
        {
            PopulateComponentList();
        }
        
        private void PopulateComponentList()
        {
            var componentNamespace = typeof(Component1).Namespace;
            var componentTypes = typeof(Component1).Assembly.GetTypes().Where(x => x.Namespace == componentNamespace);
            _componentTypes = componentTypes.ToList();
        }

        public IEnumerable<IGroup> CreateTestGroups(int cycles = 5)
        {
            for (var i = 1; i < cycles; i++)
            {
                for (var j = 0; j < _componentTypes.Count; j++)
                {
                    yield return new Group(_componentTypes.Skip(i).Take(j).ToArray());
                }
            }
        }
    }
}