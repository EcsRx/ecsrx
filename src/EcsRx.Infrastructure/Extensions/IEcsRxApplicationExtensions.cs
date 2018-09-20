using System.ComponentModel;
using System.Linq;
using EcsRx.Executor;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Systems;
using EcsRx.Views.Systems;

namespace EcsRx.Infrastructure.Extensions
{
    public static class IEcsRxApplicationExtensions
    {
        /// <summary>
        /// This will bind the given system type (T) to the DI container against `ISystem`
        /// and will then immediately register the system with the SystemExecutor.
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <typeparam name="T">The implementation of ISystem to bind/register</typeparam>
        public static void BindAndRegisterSystem<T>(this IEcsRxApplication application) where T : ISystem
        {
            application.Container.Bind<ISystem, T>(new BindingConfiguration{WithName = typeof(T).Name});
            RegisterSystem<T>(application);
        }

        /// <summary>
        /// This will resolve the given type (T) from the DI container then register it
        /// with the SystemExecutor.
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <typeparam name="T">The implementation of ISystem to register</typeparam>
        public static void RegisterSystem<T>(this IEcsRxApplication application) where T : ISystem
        {
            ISystem system;
            
            if(application.Container.HasBinding<ISystem>(typeof(T).Name))
            { system = application.Container.Resolve<ISystem>(typeof(T).Name); }
            else
            { system = application.Container.Resolve<T>(); }
            
            application.SystemExecutor.AddSystem(system);
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        public static void RegisterAllBoundSystems(this IEcsRxApplication application)
        {
            var allSystems = application.Container.ResolveAll<ISystem>();

            var orderedSystems = allSystems
                .OrderByDescending(x => x is ViewResolverSystem)
                .ThenByDescending(x => x is ISetupSystem);
            
            orderedSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
    }
}