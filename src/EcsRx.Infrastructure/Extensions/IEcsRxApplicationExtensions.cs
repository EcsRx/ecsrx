using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Systems;

namespace EcsRx.Infrastructure.Extensions
{
    public static class IEcsRxApplicationExtensions
    {
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
            var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var systemType = typeof(ISystem);           
            
            var applicableSystems = applicationAssemblies.SelectMany(x => x.GetTypes())
                .Where(x =>
                {                   
                    if(x.IsInterface || x.IsAbstract)
                    { return false; }
                    
                    if(string.IsNullOrEmpty(x.Namespace))
                    { return false; }   
                    
                    if(!systemType.IsAssignableFrom(x))
                    { return false; }

                    return namespaces.Any(namespaceToVerify => x.Namespace.Contains(namespaceToVerify));
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
        
        /// <summary>
        /// This will bind the given system type (T) to the DI container against `ISystem`
        /// and will then immediately register the system with the SystemExecutor.
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <typeparam name="T">The implementation of ISystem to bind/register</typeparam>
        /// <remarks>This is really for runtime usage, in mose cases you will want to bind in starting and register in started</remarks>
        public static void BindAndStartSystem<T>(this IEcsRxApplication application) where T : ISystem
        {
            application.Container.Bind<ISystem, T>(new BindingConfiguration{WithName = typeof(T).Name});
            StartSystem<T>(application);
        }

        /// <summary>
        /// This will resolve the given type (T) from the DI container then register it
        /// with the SystemExecutor.
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <typeparam name="T">The implementation of ISystem to register</typeparam>
        public static void StartSystem<T>(this IEcsRxApplication application) where T : ISystem
        {
            ISystem system;
            
            if(application.Container.HasBinding<ISystem>(typeof(T).Name))
            { system = application.Container.Resolve<ISystem>(typeof(T).Name); }
            else
            { system = application.Container.Resolve<T>(); }
            
            application.SystemExecutor.AddSystem(system);
        }
        
        /// <summary>
        /// Resolve all systems which have been bound in the order they should be registered
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>This ordering will be purely by priority</remarks>
        public static IEnumerable<ISystem> GetAllBoundSystems(this IEcsRxApplication application)
        {
            var allSystems = application.Container.ResolveAll<ISystem>();
            return allSystems.OrderByPriority();
        }
        
        /// <summary>
        /// Resolve all systems which have been bound and register them in order with the systems executor
        /// </summary>
        /// <param name="application">The application to act on</param>
        /// <remarks>Will be purely</remarks>
        public static void StartAllBoundSystems(this IEcsRxApplication application)
        {
            var allSystems = GetAllBoundSystems(application);
            allSystems.ForEachRun(application.SystemExecutor.AddSystem);
        }
    }
}