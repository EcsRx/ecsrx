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
            var requiredComponents = new List<Type>(group.RequiredComponents) {typeof(T)};

            return group is IHasPredicate ? 
                new GroupWithPredicate(null, requiredComponents, group.ExcludedComponents) : 
                new Group(requiredComponents, group.ExcludedComponents);
        }
        
        public static IGroup WithComponents(this IGroup group, params Type[] requiredComponents)
        {
            var newComponents = new List<Type>(group.RequiredComponents);
            newComponents.AddRange(requiredComponents);
            return group is IHasPredicate ? 
                new GroupWithPredicate(null, newComponents, group.ExcludedComponents) : 
                new Group(newComponents, group.ExcludedComponents);
        }
        
        public static IGroup WithoutComponent<T>(this IGroup group) where T : class, IComponent
        {
            var excludedComponents = new List<Type>(group.ExcludedComponents) {typeof(T)};
            return group is IHasPredicate ? 
                new GroupWithPredicate(null, group.RequiredComponents, excludedComponents) : 
                new Group(group.RequiredComponents, excludedComponents);
        }
        
        public static IGroup WithoutComponent(this IGroup group, params Type[] excludedComponents)
        {
            var newComponents = new List<Type>(group.ExcludedComponents);
            newComponents.AddRange(excludedComponents);
            
            return group is IHasPredicate ? 
                new GroupWithPredicate(null, group.RequiredComponents, newComponents) : 
                new Group(group.RequiredComponents, newComponents);
        }
        
        public static bool ContainsAllRequiredComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!components.Any(x => group.RequiredComponents[i].IsInstanceOfType(x)))
                { return false; }
            }
            return true;
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
       
        public static bool ContainsAllRequiredComponents(this IGroup group, IEntity entity)
        { return entity.HasAllComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyRequiredComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(components.Any(x => group.RequiredComponents[i] == x.GetType()))
                { return true; }
            }
            return false;
        }

        public static bool ContainsAnyRequiredComponents(this IGroup group, params Type[] componentTypes)
        {

            for (var i = @group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                for (var j = componentTypes.Length - 1; j >= 0; j--)
                {
                    if(group.RequiredComponents[i] == componentTypes[j])
                    { return true; }
                }
            }

            return false;
        }
        
        public static bool ContainsAnyRequiredComponents(this IGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, IEnumerable<IComponent> components)
        {
            var castComponents = components.Select(x => x.GetType()).ToArray();
            return ContainsAnyExcludedComponents(group, castComponents);
        }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, params Type[] componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }
        
        public static bool ContainsAnyExcludedComponents(this IGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.ExcludedComponents); }

        public static bool ContainsAny(this IGroup group, params IComponent[] components)
        {
            var requiredContains = group.ContainsAnyRequiredComponents(components);
            if(requiredContains) { return true; }

            return group.ContainsAnyExcludedComponents(components);
        }
        
        public static bool ContainsAny(this IGroup group, params Type[] componentTypes)
        {
            var requiredContains = group.RequiredComponents.Any(componentTypes.Contains);
            
            if(requiredContains) { return true; }

            return group.ExcludedComponents.Any(componentTypes.Contains);
        }
        
        public static bool ContainsAny(this IGroup group, IEntity entity)
        {
            var requiredContains = group.ContainsAnyRequiredComponents(entity);
            if(requiredContains) { return true; }

            return group.ContainsAnyExcludedComponents(entity);
        }

        public static bool Matches(this IGroup group, IEntity entity)
        {
            if(group.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(group, entity); }
            
            return ContainsAllRequiredComponents(group, entity) && !ContainsAnyExcludedComponents(group, entity);
        }
    }
}