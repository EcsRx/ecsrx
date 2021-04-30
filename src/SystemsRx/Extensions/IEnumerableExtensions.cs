using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SystemsRx.Attributes;
using SystemsRx.Systems;

namespace SystemsRx.Extensions
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