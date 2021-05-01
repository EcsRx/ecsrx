using System;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Attributes;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;

namespace SystemsRx.Extensions
{
    public static class ISystemExtensions
    {
        public static bool ShouldMutliThread(this ISystem system)
        {
            return system.GetType()
                       .GetCustomAttributes(typeof(MultiThreadAttribute), true)
                       .FirstOrDefault() != null;
        }
        
        public static IEnumerable<Type> GetGenericInterfacesFor(this ISystem system, Type systemType)
        {
            return system.GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == systemType);
        }
        
        public static bool MatchesSystemTypeWithGeneric(this ISystem system, Type systemType)
        { return GetGenericInterfacesFor(system, systemType).Any(); }
        
        public static Type GetGenericDataType(this ISystem system, Type systemType)
        {
            var matchingInterface = GetGenericInterfaceType(system, systemType);
            return matchingInterface.GetGenericArguments()[0];
        }

        public static Type GetGenericInterfaceType(this ISystem system, Type systemType)
        { return system.GetType().GetMatchingInterfaceGenericTypes(systemType); }
        
        public static bool IsReactToEventSystem(this ISystem system)
        { return system.MatchesSystemTypeWithGeneric(typeof(IReactToEventSystem<>)); }
    }
}