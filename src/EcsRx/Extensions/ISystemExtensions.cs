using System;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Extensions
{
    public static class ISystemExtensions
    {
        public static IGroup GroupFor(this IGroupSystem groupSystem, params Type[] requiredTypes)
        { return new Group(requiredTypes); }
        
        public static int[] GetGroupAffinities(this IGroupSystem groupSystem)
        {
            var affinity = groupSystem.GetType()
                .GetCustomAttributes(typeof(CollectionAffinityAttribute), true)
                .FirstOrDefault();

            return ((CollectionAffinityAttribute) affinity)?.CollectionIds;
        }
    }
}