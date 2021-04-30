using SystemsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.Persistence;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Plugins.Views;

namespace EcsRx.Examples.Application
{
    public abstract class EcsRxConsoleApplication : EcsRxApplication
    {
        public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ReactiveSystemsPlugin());
            RegisterPlugin(new ComputedsPlugin());
            RegisterPlugin(new ViewsPlugin());
            RegisterPlugin(new BatchPlugin());
            RegisterPlugin(new PersistencePlugin());
        }

        protected override void StartSystems()
        {
            this.StartAllBoundReactiveSystems();
        }
    }
}