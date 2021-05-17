using System;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;
using SystemsRx.Systems;

namespace EcsRx.Extensions
{
    public static class ISystemExtensions
    {
        public static IGroup GroupFor(this IGroupSystem groupSystem, params Type[] requiredTypes)
        { return new Group(requiredTypes); }
        
        public static int[] GetGroupAffinities(this ISystem system)
        {
            if (system is null)
            {
                throw new ArgumentNullException(nameof(system));
            }

            var affinity = system.GetType()
                .GetCustomAttributes(typeof(CollectionAffinityAttribute), true)
                .FirstOrDefault();

            return ((CollectionAffinityAttribute) affinity)?.CollectionIds;
        }
    }
}