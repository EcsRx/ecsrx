using System;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Extensions
{
    public static class ISystemExtensions
    {
        public static IGroup GroupFor(this ISystem system, params Type[] requiredTypes)
        { return new Group(requiredTypes); }
        
        public static bool ShouldMutliThread(this ISystem system)
        {
            return system.GetType()
                       .GetCustomAttributes(typeof(MultiThreadAttribute), true)
                       .FirstOrDefault() != null;
        }
    }
}