using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Modules;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Systems;

namespace EcsRx.Infrastructure
{
    public abstract class EcsRxApplication : IEcsRxApplication
    {
        public ISystemExecutor SystemExecutor { get; private set; }
        public IEventSystem EventSystem { get; private set; }
        public IEntityCollectionManager EntityCollectionManager { get; private set; }
        public IEnumerable<IEcsRxPlugin> Plugins => _plugins;

        private readonly List<IEcsRxPlugin> _plugins;

        public abstract IDependencyContainer Container { get; }

        protected EcsRxApplication()
        {
            _plugins = new List<IEcsRxPlugin>();
        }

        /// <summary>
        /// This starts the process of initializing the application
        /// </summary>
        public virtual void StartApplication()
        {
            LoadModules();
            LoadPlugins();
            SetupPlugins();
            ResolveApplicationDependencies();
            BindSystems();
            StartPluginSystems();
            StartSystems();
            ApplicationStarted();
        }

        public virtual void StopApplication()
        { StopAndUnbindAllSystems(); }

        /// <summary>
        /// Load any modules that your application needs
        /// </summary>
        /// <remarks>
        /// If you wish to use the default setup call through to base, if you wish to stop the default framework
        /// modules loading then do not call base and register your own internal framework module.
        /// </remarks>
        protected virtual void LoadModules()
        {
            Container.LoadModule(new FrameworkModule());
        }

        /// <summary>
        /// Load any plugins that your application needs
        /// </summary>
        /// <remarks>It is recommended you just call RegisterPlugin method in here for each plugin you need</remarks>
        protected virtual void LoadPlugins(){}

        /// <summary>
        /// Resolve any dependencies the application needs
        /// </summary>
        /// <remarks>By default it will setup SystemExecutor, EventSystem, EntityCollectionManager</remarks>
        protected virtual void ResolveApplicationDependencies()
        {
            SystemExecutor = Container.Resolve<ISystemExecutor>();
            EventSystem = Container.Resolve<IEventSystem>();
            EntityCollectionManager = Container.Resolve<IEntityCollectionManager>();
        }

        /// <summary>
        /// Bind any systems that the application will need
        /// </summary>
        /// <remarks>By default will auto bind any systems within application scope</remarks>
        protected virtual void BindSystems()
        { this.BindAllSystemsWithinApplicationScope(); }

        protected virtual void StopAndUnbindAllSystems()
        {
            var allSystems = SystemExecutor.Systems.ToList();
            allSystems.ForEachRun(SystemExecutor.RemoveSystem);
            Container.Unbind<ISystem>();
        }

        /// <summary>
        /// Start any systems that the application will need
        /// </summary>
        /// <remarks>By default it will auto start any systems which have been bound</remarks>
        protected virtual void StartSystems()
        { this.StartAllBoundSystems(); }
        
        protected abstract void ApplicationStarted();

        protected void SetupPlugins()
        { Plugins.ForEachRun(x => x.SetupDependencies(Container)); }

        protected void StartPluginSystems()
        {
            Plugins.SelectMany(x => x.GetSystemsForRegistration(Container))
                .ForEachRun(x => SystemExecutor.AddSystem(x));
        }

        /// <summary>
        /// Register a given plugin within the application
        /// </summary>
        /// <param name="plugin">The plugin to register</param>
        protected void RegisterPlugin(IEcsRxPlugin plugin)
        { _plugins.Add(plugin); }
    }
}