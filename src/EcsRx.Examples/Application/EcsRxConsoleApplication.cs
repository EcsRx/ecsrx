using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Ninject;

namespace EcsRx.Examples.Application
{
    public abstract class EcsRxConsoleApplication : EcsRxApplication
    {
        public override IDependencyContainer Container { get; } = new NinjectDependencyContainer();
    }
}