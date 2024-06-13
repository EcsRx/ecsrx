using System.Linq;
using SystemsRx.Events;
using SystemsRx.Executor;
using SystemsRx.Executor.Handlers;
using SystemsRx.Executor.Handlers.Conventional;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Pools;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Groups.Observable;
using EcsRx.Groups.Observable.Tracking;
using EcsRx.Systems.Handlers;

namespace EcsRx.Examples.ExampleApps.Performance.Modules
{
    public class OptimizedEcsRxInfrastructureModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<IIdPool, IdPool>();
            registry.Bind<IEntityFactory, DefaultEntityFactory>();
            registry.Bind<IEntityCollectionFactory, DefaultEntityCollectionFactory>();
            registry.Bind<IEntityDatabase, EntityDatabase>();
            registry.Bind<IObservableGroupFactory, DefaultObservableObservableGroupFactory>();
            registry.Bind<IObservableGroupManager, ObservableGroupManager>();
            registry.Bind<IConventionalSystemHandler, BasicEntitySystemHandler>();
            registry.Bind<IComponentTypeAssigner, DefaultComponentTypeAssigner>();
            registry.Bind<IGroupTrackerFactory, GroupTrackerFactory>();
            
            var componentNamespace = typeof(Component1).Namespace;
            var componentTypes = typeof(Component1).Assembly.GetTypes().Where(x => x.Namespace == componentNamespace);
            var explicitTypeLookups = componentTypes.Select((type, i) => new { type, i }).ToDictionary(x => x.type, x => x.i);
            var explicitComponentLookup = new ComponentTypeLookup(explicitTypeLookups);

            registry.Bind<IComponentTypeLookup>(new BindingConfiguration{ToInstance = explicitComponentLookup});           
            registry.Bind<IComponentDatabase, ComponentDatabase>();
        }
    }
}