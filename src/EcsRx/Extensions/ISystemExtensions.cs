using System;
using System.Reflection;
using EcsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;
using SystemsRx.Extensions;
using SystemsRx.Systems;

namespace EcsRx.Extensions
{
    public static class ISystemExtensions
    {
        public static IGroup GroupFor(this IGroupSystem groupSystem, params Type[] requiredTypes)
        { return new Group(requiredTypes); }
        
        public static int[] GetGroupAffinities(this IGroupSystem system)
        {
            if (system is null)
            { throw new ArgumentNullException(nameof(system)); }

            var affinity = system.GetType().GetCustomAttribute(typeof(CollectionAffinityAttribute), true);

            return ((CollectionAffinityAttribute) affinity)?.CollectionIds;
        }

        public static int[] GetGroupAffinities(this ISystem system, MemberInfo memberInfo)
        {
            if (memberInfo is null)
            { throw new ArgumentNullException(nameof(memberInfo)); }

            var collectionAffinityAttribute = (CollectionAffinityAttribute)memberInfo.GetCustomAttribute(typeof(CollectionAffinityAttribute), true);
            return collectionAffinityAttribute?.CollectionIds;
        }
        
        public static bool IsSystemReactive(this ISystem system)
        { return system is IReactToEntitySystem || system is IReactToGroupSystem || system.IsReactiveDataSystem(); }

        public static bool IsReactiveDataSystem(this ISystem system)
        { return system.MatchesSystemTypeWithGeneric(typeof(IReactToDataSystem<>)); }
    }
}