using System;
using System.Collections.Generic;
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
            
            var explicitTypeLookups = new Dictionary<Type, int>
            {
                {typeof(Component1), 0},
                {typeof(Component2), 1},
                {typeof(Component3), 2},
                {typeof(Component4), 3},
                {typeof(Component5), 4},
                {typeof(Component6), 5},
                {typeof(Component7), 6},
                {typeof(Component8), 7},
                {typeof(Component9), 8},
                {typeof(Component10), 9},
                {typeof(Component11), 10},
                {typeof(Component12), 11},
                {typeof(Component13), 12},
                {typeof(Component14), 13},
                {typeof(Component15), 14},
                {typeof(Component16), 15},
                {typeof(Component17), 16},
                {typeof(Component18), 17},
                {typeof(Component19), 18},
                {typeof(Component20), 19}
            };

            var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);

            container.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});           
            container.Bind<IComponentDatabase, ComponentDatabase>();
            container.Bind<IComponentRepository, ComponentRepository>();
        }
    }
}