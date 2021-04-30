using SystemsRx.Extensions;
using SystemsRx.Systems;
using EcsRx.Plugins.ReactiveSystems.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Extensions
{
    public static class ISystemExtensions
    {
        public static bool IsSystemReactive(this ISystem system)
        { return system is IReactToEntitySystem || system is IReactToGroupSystem || system.IsReactiveDataSystem(); }

        public static bool IsReactiveDataSystem(this ISystem system)
        { return system.MatchesSystemTypeWithGeneric(typeof(IReactToDataSystem<>)); }
    }
}