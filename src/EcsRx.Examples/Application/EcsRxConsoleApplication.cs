using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Ninject;
using EcsRx.Infrastructure;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Persistence;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Plugins.Views;

namespace EcsRx.Examples.Application
{
    public abstract class EcsRxConsoleApplication : EcsRxApplication
    {
        public override IDependencyRegistry DependencyRegistry { get; } = new NinjectDependencyRegistry();

        protected override void LoadPlugins()
        {
            RegisterPlugin(new ReactiveSystemsPlugin());
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