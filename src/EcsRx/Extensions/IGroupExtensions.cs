using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class IGroupExtensions
    {
        public static IGroup WithComponent<T>(this IGroup group) where T : class, IComponent
        {
            var componentTypes = new List<Type>(group.WithComponents);
            componentTypes.Add(typeof(T));
            return new Group(componentTypes.ToArray());
        }
        
        public static bool ContainsAll(this IGroup group, IComponent[] components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            for (var i = components.Length - 1; i >= 0; i--)
            {
                if (!group.WithComponents.Contains(castComponents[i]))
                { return false; }
            }
            return true;
        }

        public static bool ContainsAny(this IGroup group, IComponent[] components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return group.WithComponents.Any(x => castComponents.Contains(x));
        }
    }
}