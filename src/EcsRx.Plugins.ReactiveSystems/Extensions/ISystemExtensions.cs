using System;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Extensions
{
    public static class ISystemExtensions
    {
        public static bool IsSystemReactive(this ISystem system)
        { return system is IReactToEntitySystem || system is IReactToGroupSystem || system.IsReactiveDataSystem(); }

        public static bool IsReactiveDataSystem(this ISystem system)
        {
            return system.GetType()
                .GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IReactToDataSystem<>));
        }

        public static Type GetGenericDataType(this ISystem system)
        {
            var matchingInterface = system.GetType()
                .GetInterfaces()
                .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IReactToDataSystem<>));

            return matchingInterface.GetGenericArguments()[0];
        }

        public static Type GetGenericInterfaceType(this ISystem system)
        {
            var matchingInterface = system.GetType()
                .GetInterfaces()
                .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReactToDataSystem<>));

            return matchingInterface;
        }
        
        public static int[] GetGroupAffinities(this ISystem system)
        {
            var affinity = system.GetType()
                .GetCustomAttributes(typeof(CollectionAffinityAttribute), true)
                .FirstOrDefault();

            return ((CollectionAffinityAttribute) affinity)?.CollectionIds;
        }
    }
}