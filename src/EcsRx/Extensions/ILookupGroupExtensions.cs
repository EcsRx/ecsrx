using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class ILookupGroupExtensions
    {                
        public static bool ContainsAllRequiredComponents(this ILookupGroup group, params int[] componentTypeIds)
        { return ContainsAllRequiredComponents(group, (IReadOnlyList<int>) componentTypeIds); }
        
        public static bool ContainsAllRequiredComponents(this ILookupGroup group, IReadOnlyList<int> componentTypeIds)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!componentTypeIds.Contains(group.RequiredComponents[i]))
                { return false; }
            }
            
            return true;
        }
        
        public static bool ContainsAllRequiredComponents(this ILookupGroup group, IEntity entity)
        { return entity.HasAllComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyRequiredComponents(this ILookupGroup group, params int[] componentTypes)
        { return ContainsAnyRequiredComponents(group, (IReadOnlyList<int>) componentTypes); }
        
        public static bool ContainsAnyRequiredComponents(this ILookupGroup group, IReadOnlyList<int> componentTypes)
        {
            for (var i = @group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                for (var j = componentTypes.Count - 1; j >= 0; j--)
                {
                    if(group.RequiredComponents[i] == componentTypes[j])
                    { return true; }
                }
            }

            return false;
        }
        
        public static bool ContainsAnyRequiredComponents(this ILookupGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyExcludedComponents(this ILookupGroup group, params int[] componentTypes)
        { return ContainsAnyExcludedComponents(group, (IReadOnlyList<int>)componentTypes); }
        
        public static bool ContainsAnyExcludedComponents(this ILookupGroup group, IReadOnlyList<int> componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }
        
        public static bool ContainsAnyExcludedComponents(this ILookupGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.ExcludedComponents); }

        public static bool ContainsAny(this ILookupGroup group, params int[] components)
        { return ContainsAny(group, (IReadOnlyList<int>) components); }
        
        public static bool ContainsAny(this ILookupGroup group, IReadOnlyList<int> components)
        {
            var requiredContains = group.ContainsAnyRequiredComponents(components);
            if(requiredContains) { return true; }

            return group.ContainsAnyExcludedComponents(components);
        }
        
        public static bool Matches(this ILookupGroup lookupGroup, IEntity entity)
        {
            if(lookupGroup.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(lookupGroup, entity); }
            
            return ContainsAllRequiredComponents(lookupGroup, entity) && !ContainsAnyExcludedComponents(lookupGroup, entity);
        }
        
        public static bool Matches(this ILookupGroup lookupGroup, params int[] componentTypes)
        { return Matches(lookupGroup, (IReadOnlyList<int>) componentTypes); }
        
        public static bool Matches(this ILookupGroup lookupGroup, IReadOnlyList<int> componentTypes)
        {
            if(lookupGroup.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(lookupGroup, componentTypes); }
            
            return ContainsAllRequiredComponents(lookupGroup, componentTypes) && !ContainsAnyExcludedComponents(lookupGroup, componentTypes);
        }
    }
}