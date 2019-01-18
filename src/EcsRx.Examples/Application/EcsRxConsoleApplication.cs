using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Batching;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.Views;
using EcsRx.Plugins.Views.Extensions;

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
        }

        protected override void StartSystems()
        {
            this.StartAllBoundSystems();
        }
    }
}