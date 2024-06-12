using SystemsRx.Executor.Handlers;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using SystemsRx.Pools;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Groups.Observable;
using EcsRx.Groups.Observable.Tracking;
using EcsRx.Systems.Handlers;

namespace EcsRx.Infrastructure.Modules
{
    public class EcsRxInfrastructureModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            // Register ECS specific infrastructure
            registry.Bind<IIdPool, IdPool>();
            registry.Bind<IEntityFactory, DefaultEntityFactory>();
            registry.Bind<IEntityCollectionFactory, DefaultEntityCollectionFactory>();
            registry.Bind<IEntityDatabase, EntityDatabase>();
            registry.Bind<IObservableGroupFactory, DefaultObservableObservableGroupFactory>();
            registry.Bind<IObservableGroupManager, ObservableGroupManager>();
            registry.Bind<IComponentTypeAssigner, DefaultComponentTypeAssigner>();
            registry.Bind<IComponentTypeLookup>(new BindingConfiguration{ToMethod = CreateDefaultTypeLookup});           
            registry.Bind<IComponentDatabase, ComponentDatabase>();
            registry.Bind<IGroupTrackerFactory, GroupTrackerFactory>();
            
            // Register ECS specific system handlers
            registry.Bind<IConventionalSystemHandler, BasicEntitySystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToEntitySystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToGroupSystemHandler>();
            registry.Bind<IConventionalSystemHandler, ReactToDataSystemHandler>();
            registry.Bind<IConventionalSystemHandler, SetupSystemHandler>();
            registry.Bind<IConventionalSystemHandler, TeardownSystemHandler>();
        }

        private static object CreateDefaultTypeLookup(IDependencyResolver resolver)
        {
            var componentTypeAssigner = resolver.Resolve<IComponentTypeAssigner>();
            var allComponents = componentTypeAssigner.GenerateComponentLookups();
            return new ComponentTypeLookup(allComponents);
        }
    }
}