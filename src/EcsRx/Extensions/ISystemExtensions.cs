using System;
using System.Linq;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Extensions
{
    public static class ISystemExtensions
    {
        public static IGroup GroupFor(this ISystem system, params Type[] requiredTypes)
        { return new Group(requiredTypes); }
    }
}