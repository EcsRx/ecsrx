using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class IGroupExtensions
    {
        public static IGroup WithComponent<T>(this IGroup group) where T : class, IComponent
        {
            var componentTypes = new List<Type>(group.RequiredComponents) {typeof(T)};
            return new Group(componentTypes.ToArray());
        }
        
        public static bool ContainsAllRequiredComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return ContainsAllRequiredComponents(group, castComponents);
        }
        
        public static bool ContainsAllRequiredComponents(this IGroup group, params Type[] componentTypes)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!componentTypes.Contains(group.RequiredComponents[i]))
                { return false; }
            }
            
            return true;
        }
        
        public static bool ContainsAnyRequiredComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return ContainsAnyRequiredComponents(group, castComponents);
        }
        
        public static bool ContainsAnyRequiredComponents(this IGroup group, params Type[] componentTypes)
        { return group.RequiredComponents.Any(componentTypes.Contains); }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return ContainsAnyExcludedComponents(group, castComponents);
        }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, params Type[] componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }

        public static bool ContainsAny(this IGroup group, params IComponent[] components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return ContainsAny(group, castComponents);
        }
        
        public static bool ContainsAny(this IGroup group, params Type[] componentTypes)
        {
            var requiredContains = group.RequiredComponents.Any(componentTypes.Contains);
            
            if(requiredContains) { return true; }

            return group.ExcludedComponents.Any(componentTypes.Contains);
        }

        public static bool Matches(this IGroup group, IEntity entity)
        {
            if(group.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(group, entity.Components); }
            
            return ContainsAllRequiredComponents(group, entity.Components) && !ContainsAnyExcludedComponents(group, entity.Components);
        }
    }
}