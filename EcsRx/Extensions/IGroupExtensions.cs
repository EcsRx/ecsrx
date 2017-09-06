using System;
using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class IGroupExtensions
    {
        public static IGroup WithComponent<T>(this IGroup group) where T : class, IComponent
        {
            var componentTypes = new List<Type>(group.TargettedComponents);
            componentTypes.Add(typeof(T));
            return new Group(componentTypes.ToArray());
        }
    }
}