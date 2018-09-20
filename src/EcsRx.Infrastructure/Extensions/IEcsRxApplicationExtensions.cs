using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        
        /// <summary>
        /// This will bind any ISystem implementations that are found within the assembly provided
        /// </summary>
        /// <remarks>
        /// This can save you time but is not advised in most cases
        /// </remarks>
        /// <param name="application">The application to act on</param>
        /// <param name="assemblies">The assemblies to scan for systems</param>
        public static void BindAllSystemsInAssemblies(this IEcsRxApplication application, params Assembly[] assemblies)
        {           
            var systemType = typeof(ISystem);           
            
            var applicableSystems = assemblies.SelectMany(x => x.GetTypes())
                .Where(x =>
                {                   
                    if(x.IsInterface || x.IsAbstract)
                    { return false; }
                                        
                    return systemType.IsAssignableFrom(x);
                })
                .ToList();

            if(!applicableSystems.Any())
            { return; }

            foreach (var applicableSystemType in applicableSystems)
            {
                var bindingConfiguration = new BindingConfiguration
                {
                    AsSingleton = true,
                    WithName = applicableSystemType.Name
                };
                
                application.Container.Bind(systemType, applicableSystemType,  bindingConfiguration);
            }
        }

        /// <summary>
        /// This will bind any ISystem implementations that are found within the namespaces provided
        /// </summary>
        /// <remarks>
        /// It is also advised you wrap this method with your own conventions like BindAllSystemsWithinApplicationScope does.
        /// </remarks>
        /// <param name="application">The application to act on</param>
        /// <param name="namespaces">The namespaces to be scanned for implementations</param>
        public static void BindAllSystemsInNamespaces(this IEcsRxApplication application, params string[] namespaces)
        {
            var applicationNamespace = application.GetType().Namespace;
            if(string.IsNullOrEmpty(applicationNamespace))
            { return; }
            
            var applicationAssembly = application.GetType().Assembly;
            var systemType = typeof(ISystem);           
            
            var applicableSystems = applicationAssembly.GetTypes()
                .Where(x =>
                {                   
                    if(x.IsInterface || x.IsAbstract)
                    { return false; }
                    
                    if(string.IsNullOrEmpty(x.Namespace) || !x.Namespace.Contains(applicationNamespace))
                    { return false; }   
                    
                    return systemType.IsAssignableFrom(x);
                })
                .ToList();

            if(!applicableSystems.Any())
            { return; }

            foreach (var applicableSystemType in applicableSystems)
            {
                var bindingConfiguration = new BindingConfiguration
                {
                    AsSingleton = true,
                    WithName = applicableSystemType.Name
                };
                
                application.Container.Bind(systemType, applicableSystemType,  bindingConfiguration);
            }
        }
        
        /// <summary>
        /// This will bind any ISystem implementations found within Systems, ViewResolvers folders which are located
        /// in a child namespace of the application.
        /// </summary>
        /// <remarks>
        /// This is a conventional based binding that expects the application file to sit in the root of a directory,
        /// and then have systems in folders within same directory,if you need other conventions then look at wrapping
        /// BindAnySystemsInNamespace
        /// </remarks>
        /// <param name="application">The application to act on</param>
        public static void BindAllSystemsWithinApplicationScope(this IEcsRxApplication application)
        {
            var applicationNamespace = application.GetType().Namespace;
            var namespaces = new[]
            {
                $"{applicationNamespace}.Systems",
                $"{applicationNamespace}.ViewResolvers"
            };
            
            application.BindAllSystemsInNamespaces(namespaces);
        }
    }
}