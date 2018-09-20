using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Modules;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Systems;
using EcsRx.Views.Systems;

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

        public virtual void StartApplication()
        {
            RegisterModules();
            ApplicationStarting();
            RegisterAllPluginDependencies();
            SetupAllPluginSystems();
            ApplicationStarted();
        }

        protected virtual IDependencyModule GetFrameworkModule()
        {
            return new FrameworkModule();
        }

        protected virtual void RegisterModules()
        {
            Container.LoadModule(GetFrameworkModule());
        }

        private void ResolveDependencies()
        {
            SystemExecutor = Container.Resolve<ISystemExecutor>();
            EventSystem = Container.Resolve<IEventSystem>();
            EntityCollectionManager = Container.Resolve<IEntityCollectionManager>();
        }

        protected virtual void ApplicationStarting() { }
        protected abstract void ApplicationStarted();

        protected virtual void RegisterAllPluginDependencies()
        { Plugins.ForEachRun(x => x.SetupDependencies(Container)); }

        protected virtual void SetupAllPluginSystems()
        {
            Plugins.SelectMany(x => x.GetSystemsForRegistration(Container))
                .ForEachRun(x => SystemExecutor.AddSystem(x));
        }

        protected void RegisterPlugin(IEcsRxPlugin plugin)
        { _plugins.Add(plugin); }
    }
}