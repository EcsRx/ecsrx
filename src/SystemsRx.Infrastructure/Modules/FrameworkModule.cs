using SystemsRx.Events;
using SystemsRx.Executor;
using SystemsRx.Executor.Handlers;
using SystemsRx.Executor.Handlers.Conventional;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Scheduling;
using SystemsRx.Threading;
using EcsRx.MicroRx.Events;

namespace SystemsRx.Infrastructure.Modules
{
    public class FrameworkModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IMessageBroker, MessageBroker>();
            container.Bind<IEventSystem, EventSystem>();
            container.Bind<IThreadHandler, DefaultThreadHandler>();
            container.Bind<IConventionalSystemHandler, ManualSystemHandler>();
            container.Bind<IConventionalSystemHandler, BasicSystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToEventSystemHandler>();
            container.Bind<ISystemExecutor, SystemExecutor>();
            container.Bind<IUpdateScheduler, DefaultUpdateScheduler>();
            container.Bind<ITimeTracker>(x => x.ToBoundType(typeof(IUpdateScheduler)));
        }
    }
}