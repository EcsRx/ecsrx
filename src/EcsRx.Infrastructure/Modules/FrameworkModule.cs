using EcsRx.Collections;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Events;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Scheduling;
using EcsRx.MicroRx.Events;
using EcsRx.Pools;
using EcsRx.Threading;

namespace EcsRx.Infrastructure.Modules
{
    public class FrameworkModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IMessageBroker, MessageBroker>();
            container.Bind<IEventSystem, EventSystem>();
            container.Bind<IIdPool, IdPool>();
            container.Bind<IThreadHandler, DefaultThreadHandler>();
            container.Bind<IEntityFactory, DefaultEntityFactory>();
            container.Bind<IEntityCollectionFactory, DefaultEntityCollectionFactory>();
            container.Bind<IObservableGroupFactory, DefaultObservableObservableGroupFactory>();
            container.Bind<IEntityCollectionManager, EntityCollectionManager>();
            container.Bind<IObservableGroupManager>(x => x.ToMethod(y => y.Resolve<IEntityCollectionManager>()));
            container.Bind<IConventionalSystemHandler, ManualSystemHandler>();
            container.Bind<ISystemExecutor, SystemExecutor>();
            container.Bind<IObservableScheduler, DefaultObservableScheduler>();
            container.Bind<IComponentTypeAssigner, DefaultComponentTypeAssigner>();
            container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToMethod = CreateDefaultTypeLookup});           
            container.Bind<IComponentDatabase, ComponentDatabase>();
        }

        private static object CreateDefaultTypeLookup(IDependencyContainer container)
        {
            var componentTypeAssigner = container.Resolve<IComponentTypeAssigner>();
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            return new ComponentTypeLookup(allComponents);
        }
    }
}