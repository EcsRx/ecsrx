using System;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Executor.Handlers;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Reactive;

namespace EcsRx.Infrastructure.Modules
{
    public class FrameworkModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IMessageBroker, MessageBroker>();
            container.Bind<IEventSystem, EventSystem>();
            container.Bind<IIdPool, IdPool>();
            container.Bind<IComponentTypeAssigner, DefaultComponentTypeAssigner>();
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
            
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            var componentLookup = new ComponentTypeLookup(allComponents);
            
            _availableComponents = allComponents.Keys
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();
            
            var componentDatabase = new ComponentDatabase(componentLookup);
            var componentRepository = new ComponentRepository(componentLookup, componentDatabase);
        }
    }
}