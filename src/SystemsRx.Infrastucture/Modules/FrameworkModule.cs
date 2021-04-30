using SystemsRx.Events;
using SystemsRx.Executor;
using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using SystemsRx.Scheduling;
using SystemsRx.Threading;
using EcsRx.MicroRx.Events;

namespace SystemsRx.Infrastucture.Modules
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
            container.Bind<ISystemExecutor, SystemExecutor>();
            container.Bind<IUpdateScheduler, DefaultUpdateScheduler>();
            container.Bind<ITimeTracker>(x => x.ToBoundType(typeof(IUpdateScheduler)));
        }
    }
}