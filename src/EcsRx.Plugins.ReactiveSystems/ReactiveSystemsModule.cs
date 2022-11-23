using EcsRx.Plugins.ReactiveSystems.Handlers;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;

namespace EcsRx.Plugins.ReactiveSystems
{
    public class ReactiveSystemsModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
            container.Bind<IConventionalSystemHandler, SetupSystemHandler>();
            container.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
        }
    }
}