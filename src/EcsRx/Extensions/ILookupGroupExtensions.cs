using System.Linq;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class ILookupGroupExtensions
    {                
        public static bool ContainsAllRequiredComponents(this ILookupGroup group, params int[] componentTypeIds)
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
        
        public static bool ContainsAnyRequiredComponents(this ILookupGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.RequiredComponents); }
        
        public static bool ContainsAnyExcludedComponents(this ILookupGroup group, params int[] componentTypes)
        { return group.ExcludedComponents.Any(componentTypes.Contains); }
        
        public static bool ContainsAnyExcludedComponents(this ILookupGroup group, IEntity entity)
        { return entity.HasAnyComponents(group.ExcludedComponents); }

        public static bool ContainsAny(this ILookupGroup group, params int[] components)
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
    }
}