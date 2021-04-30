using System.Collections.Generic;
using System.Linq;
using SystemsRx.Extensions;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Systems;
using EcsRx.Infrastructure;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Plugins.Views.Systems;

namespace EcsRx.Plugins.Views.Extensions
{
    public static class IEcsRxApplicationExtensions
    {
        /// <summary>
        /// Resolve all systems which have been bound in the order they need to be triggered
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>This ordering will be Setup, ViewResolvers, Anything Else</remarks>
        public static IEnumerable<ISystem> GetAllBoundViewSystems(this IEcsRxApplication application)
        {
            var allSystems = application.Container.ResolveAll<ISystem>();

            return allSystems
                    .OrderByDescending(x => x is ISetupSystem && !(x is IViewResolverSystem))
                    .ThenByDescending(x => x is IViewResolverSystem)
                    .ThenByPriority();
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// /// <remarks>This ordering will be Setup, ViewResolvers, Anything Else</remarks>
        public static void StartAllBoundViewSystems(this IEcsRxApplication application)
        {
            var orderedSystems = GetAllBoundViewSystems(application);
            orderedSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
    }
}