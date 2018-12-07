using EcsRx.Collections;
using EcsRx.Components;
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
using EcsRx.MicroRx;
using EcsRx.MicroRx.Events;
using EcsRx.Pools;
using EcsRx.Systems.Handlers;
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
            container.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
            container.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
            container.Bind<IConventionalSystemHandler, ManualSystemHandler>();
            container.Bind<IConventionalSystemHandler, SetupSystemHandler>();
            container.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
            container.Bind<ISystemExecutor, SystemExecutor>();
            
            var componentTypeAssigner = new DefaultComponentTypeAssigner();
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            var componentLookup = new ComponentTypeLookup(allComponents);
            
            container.Bind<IComponentTypeAssigner>(new BindingConfiguration{ToInstance = componentTypeAssigner});
            container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = componentLookup});           
            container.Bind<IComponentDatabase, ComponentDatabase>();
            container.Bind<IComponentRepository, ComponentRepository>();
        }
    }
}