using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEachRun<T>(this IEnumerable<T> enumerable, Action<T> method)
        {
            foreach (var item in enumerable)
            {
                method(item);
            }
        }

        public static IEnumerable<IEntity> MatchingGroup(this IEnumerable<IEntity> entities, IGroup group)
        { return entities.Where(group.Matches); }
        
        public static IEnumerable<IEntity> MatchingGroup(this IEnumerable<IEntity> entities, LookupGroup group)
        { return entities.Where(x => group.Matches(x)); }

        public static IEnumerable<ISystem> GetApplicableSystems(this IEnumerable<ISystem> systems, IEntity entity)
        { return systems.Where(x => entity.MatchesGroup(x.Group)); }

        public static IEnumerable<ISystem> GetApplicableSystems(this IEnumerable<ISystem> systems, IEnumerable<IComponent> components)
        {
            var componentTypes = components.Select(x => x.GetType());
            return systems.Where(x => x.Group.RequiredComponents.All(y => componentTypes.Contains(y)));
        }

        public static IOrderedEnumerable<T> OrderByPriority<T>(this IEnumerable<T> listToPrioritize)
        {
            var priorityAttributeType = typeof(PriorityAttribute);
            return listToPrioritize.OrderBy(x =>
            {
                var priorityAttributes = x.GetType().GetCustomAttributes(priorityAttributeType, true);
                if (priorityAttributes.Length <= 0) { return 0; }
                
                var priorityAttribute = priorityAttributes.FirstOrDefault() as PriorityAttribute;
                return -priorityAttribute?.Priority;
            });
        } 
        
        public static IOrderedEnumerable<T> ThenByPriority<T>(this IOrderedEnumerable<T> listToPrioritize)
        {
            var priorityAttributeType = typeof(PriorityAttribute);
            return listToPrioritize.ThenBy(x =>
            {
                var priorityAttributes = x.GetType().GetCustomAttributes(priorityAttributeType, true);
                if (priorityAttributes.Length <= 0) { return 0; }
                
                var priorityAttribute = priorityAttributes.FirstOrDefault() as PriorityAttribute;
                return -priorityAttribute?.Priority;
            });
        } 
    }
}