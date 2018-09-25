using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Plugins;

namespace EcsRx.Infrastructure
{
    /// <summary>
    /// Acts as an entry point and bootstrapper for the framework
    /// </summary>
    public interface IEcsRxApplication
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
        /// The entity collection manager, allows you to create entity collections and observable groups
        /// </summary>
        IEntityCollectionManager EntityCollectionManager { get; }
        
        /// <summary>
        /// Any plugins which have been registered within the application
        /// </summary>
        IEnumerable<IEcsRxPlugin> Plugins { get; }
        
        /// <summary>
        /// This starts the application initialization process
        /// </summary>
        void StartApplication();
    }
}