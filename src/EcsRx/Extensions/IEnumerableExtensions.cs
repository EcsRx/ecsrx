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
        {
            var componentTypes = group.MatchesComponents.ToArray();
            return entities.Where(x => x.HasComponents(componentTypes));
        }

        public static IEnumerable<ISystem> GetApplicableSystems(this IEnumerable<ISystem> systems, IEntity entity)
        { return systems.Where(x => entity.MatchesGroup(x.TargetGroup)); }

        public static IEnumerable<ISystem> GetApplicableSystems(this IEnumerable<ISystem> systems, IEnumerable<IComponent> components)
        {
            var componentTypes = components.Select(x => x.GetType());
            return systems.Where(x => x.TargetGroup.MatchesComponents.All(y => componentTypes.Contains(y)));
        }

        public static IEnumerable<T> OrderByPriority<T>(this IEnumerable<T> listToPrioritize)
        {
            return listToPrioritize.OrderBy(x =>
            {
                var finalOrder = 0;
                var priorityAttributes = x.GetType().GetCustomAttributes(typeof (PriorityAttribute), true);
                if (priorityAttributes.Length > 0)
                {
                    var priorityAttribute = priorityAttributes.FirstOrDefault() as PriorityAttribute;
                    var priority = priorityAttribute.Priority;

                    if (priority >= 0)
                    { finalOrder = int.MinValue + priority; }
                    else
                    { finalOrder -= priority; }
                }

                return finalOrder;
            });
        } 
    }
}