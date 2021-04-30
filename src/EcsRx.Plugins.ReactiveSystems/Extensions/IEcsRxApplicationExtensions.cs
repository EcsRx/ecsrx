using System.Collections.Generic;
using System.Linq;
using SystemsRx.Extensions;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Systems;
using EcsRx.Infrastructure;
using EcsRx.Plugins.ReactiveSystems.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Extensions
{
    public static class IEcsRxApplicationExtensions
    {
        /// <summary>
        /// Resolve all systems which have been bound in the order they need to be triggered
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>The ordering here will be Setup, Anything else</remarks>
        public static IEnumerable<ISystem> GetAllBoundReactiveSystems(this IEcsRxApplication application)
        {
            var allSystems = application.Container.ResolveAll<ISystem>();

            return allSystems
                .OrderByDescending(x => x is ISetupSystem)
                .ThenByPriority();
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>The ordering here will be Setup, Anything else</remarks>
        public static void StartAllBoundReactiveSystems(this IEcsRxApplication application)
        {
            var orderedSystems = GetAllBoundReactiveSystems(application);
            orderedSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
    }
}