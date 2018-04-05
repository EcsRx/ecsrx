using EcsRx.Examples.Dependencies;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Dependencies;

namespace EcsRx.Examples.Application
{
    public abstract class EcsRxConsoleApplication : EcsRxApplication
    {
        protected override IDependencyContainer DependencyContainer { get; } = new NinjectDependencyContainer();
    }
}