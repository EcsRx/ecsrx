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
            var componentTypes = new List<Type>(group.MatchesComponents);
            componentTypes.Add(typeof(T));
            return new Group(componentTypes.ToArray());
        }
        
        public static bool ContainsAllComponents(this IGroup group, IComponent[] components)
        {
            for (var i = components.Length - 1; i >= 0; i--)
            {
                if (!group.MatchesComponents.Contains(components[i].GetType()))
                { return false; }
            }
            return true;
        }
    }
}