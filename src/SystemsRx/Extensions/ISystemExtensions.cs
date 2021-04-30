using System;
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
        
        public static bool MatchesSystemTypeWithGeneric(this ISystem system, Type systemType)
        {
            return system.GetType()
                .GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == systemType);
        }
        
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