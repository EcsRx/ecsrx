using EcsRx.Plugins.ReactiveSystems.Handlers;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;

namespace EcsRx.Plugins.ReactiveSystems
{
    public class ReactiveSystemsModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
            registry.Bind<IConventionalSystemHandler, SetupSystemHandler>();
            registry.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
        }
    }
}