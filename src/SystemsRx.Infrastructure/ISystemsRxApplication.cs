using System.Collections.Generic;
using SystemsRx.Events;
using SystemsRx.Executor;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Plugins;

namespace SystemsRx.Infrastructure
{
    /// <summary>
    /// Acts as an entry point and bootstrapper for the framework
    /// </summary>
    public interface ISystemsRxApplication
    {
        /// <summary>
        /// The dependency injection container
        /// </summary>
        /// <remarks>This will abstract away the underlying DI system, in most cases you wont need it</remarks>
        IDependencyContainer Container { get; }
        
        /// <summary>
        /// The system executor, this orchestrates the systems
        /// </summary>
        ISystemExecutor SystemExecutor { get; }
        
        /// <summary>
        /// The event system to publish and subscribe to events
        /// </summary>
        IEventSystem EventSystem { get; }

        /// <summary>
        /// Any plugins which have been registered within the application
        /// </summary>
        IEnumerable<ISystemsRxPlugin> Plugins { get; }
        
        /// <summary>
        /// This starts the application initialization process
        /// </summary>
        void StartApplication();

        /// <summary>
        /// This stops the application process
        /// </summary>
        void StopApplication();
    }
}