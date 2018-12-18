using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
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

namespace EcsRx.Examples.ExampleApps.Performance.Modules
{
    public class OptimizedFrameworkModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IMessageBroker, MessageBroker>();
            container.Bind<IEventSystem, EventSystem>();
            container.Bind<IIdPool, IdPool>();
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
            
            var componentNamespace = typeof(Component1).Namespace;
            var componentTypes = typeof(Component1).Assembly.GetTypes().Where(x => x.Namespace == componentNamespace);
            var explicitTypeLookups = componentTypes.Select((type, i) => new { type, i }).ToDictionary(x => x.type, x => x.i);
            var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);

            container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});           
            container.Bind<IComponentDatabase, ComponentDatabase>();
        }
    }
}