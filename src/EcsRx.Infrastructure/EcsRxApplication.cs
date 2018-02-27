using System.Collections.Generic;
using System.Linq;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Modules;
using EcsRx.Infrastructure.Plugins;
using EcsRx.Pools;
using EcsRx.Systems;
using EcsRx.Views.Systems;

namespace EcsRx.Infrastructure
{
    public abstract class EcsRxApplication : IEcsRxApplication
    {
        public ISystemExecutor SystemExecutor { get; private set; }
        public IEventSystem EventSystem { get; private set; }
        public IPoolManager PoolManager { get; private set; }
        public List<IEcsRxPlugin> Plugins { get; }

        protected abstract IDependencyContainer DependencyContainer { get; }

        protected EcsRxApplication()
        {
            Plugins = new List<IEcsRxPlugin>();
        }

        public virtual void StartApplication()
        {
            RegisterModules();
            ApplicationStarting();
            RegisterAllPluginDependencies();
            SetupAllPluginSystems();
            ApplicationStarted();
        }

        protected virtual void RegisterModules()
        {
            DependencyContainer.LoadModule<FrameworkModule>();

            SystemExecutor = DependencyContainer.Resolve<ISystemExecutor>();
            EventSystem = DependencyContainer.Resolve<IEventSystem>();
            PoolManager = DependencyContainer.Resolve<IPoolManager>();
        }

        protected virtual void ApplicationStarting() { }
        protected abstract void ApplicationStarted();

        protected virtual void RegisterAllPluginDependencies()
        { Plugins.ForEachRun(x => x.SetupDependencies(DependencyContainer)); }

        protected virtual void SetupAllPluginSystems()
        {
            Plugins.SelectMany(x => x.GetSystemsForRegistration(DependencyContainer))
                .ForEachRun(x => SystemExecutor.AddSystem(x));
        }

        protected void RegisterPlugin(IEcsRxPlugin plugin)
        { Plugins.Add(plugin); }
        
        protected virtual void RegisterAllBoundSystems()
        {
            var allSystems = DependencyContainer.ResolveAll<ISystem>();

            var orderedSystems = allSystems
                .OrderByDescending(x => x is ViewResolverSystem)
                .ThenByDescending(x => x is ISetupSystem);
            
            orderedSystems.ForEachRun(SystemExecutor.AddSystem);
        }

        protected virtual void RegisterSystem<T>() where T : ISystem
        {
            var system = DependencyContainer.Resolve<T>();
            SystemExecutor.AddSystem(system);
        }
    }
}