using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class LookupGroupExtensions
    {                
        public static bool ContainsAllRequiredComponents(this LookupGroup group, params int[] componentTypeIds)
        { return ContainsAllRequiredComponents(group, (IReadOnlyList<int>) componentTypeIds); }
        
        public static bool ContainsAllRequiredComponents(this LookupGroup group, IReadOnlyList<int> componentTypeIds)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                if(!componentTypeIds.Contains(group.RequiredComponents[i]))
                { return false; }
            }
            
            return true;
        }
        
        public static bool ContainsAllRequiredComponents(this LookupGroup group, IEntity entity)
        { return entity.HasAllComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyRequiredComponents(this LookupGroup group, params int[] componentTypeIds)
        { return ContainsAnyRequiredComponents(group, (IReadOnlyList<int>) componentTypeIds); }
        
        public static bool ContainsAnyRequiredComponents(this LookupGroup group, IReadOnlyList<int> componentTypes)
        {
            for (var i = group.RequiredComponents.Length - 1; i >= 0; i--)
            {
                for (var j = componentTypes.Count - 1; j >= 0; j--)
                {
                    if(group.RequiredComponents[i] == componentTypes[j])
                    { return true; }
                }
            }

            return false;
        }
        
        public static bool ContainsAnyRequiredComponents(this LookupGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyExcludedComponents(this LookupGroup group, params int[] componentTypes)
        { return ContainsAnyExcludedComponents(group, (IReadOnlyList<int>)componentTypes); }
        
        public static bool ContainsAnyExcludedComponents(this LookupGroup group, IReadOnlyList<int> componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }
        
        public static bool ContainsAnyExcludedComponents(this LookupGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.ExcludedComponents); }

        public static bool ContainsAny(this LookupGroup group, params int[] components)
        { return ContainsAny(group, (IReadOnlyList<int>) components); }
        
        public static bool ContainsAny(this LookupGroup group, IReadOnlyList<int> components)
        {
            var requiredContains = group.ContainsAnyRequiredComponents(components);
            if(requiredContains) { return true; }

            return group.ContainsAnyExcludedComponents(components);
        }
        
        public static bool Matches(this LookupGroup lookupGroup, IEntity entity)
        {
            if(lookupGroup.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(lookupGroup, entity); }
            
            return ContainsAllRequiredComponents(lookupGroup, entity) && !ContainsAnyExcludedComponents(lookupGroup, entity);
        }
        
        public static bool Matches(this LookupGroup lookupGroup, params int[] componentTypes)
        { return Matches(lookupGroup, (IReadOnlyList<int>) componentTypes); }
        
        public static bool Matches(this LookupGroup lookupGroup, IReadOnlyList<int> componentTypes)
        {
            if(lookupGroup.ExcludedComponents.Length == 0)
            { return ContainsAllRequiredComponents(lookupGroup, componentTypes); }
            
            return ContainsAllRequiredComponents(lookupGroup, componentTypes) && !ContainsAnyExcludedComponents(lookupGroup, componentTypes);
        }
    }
}