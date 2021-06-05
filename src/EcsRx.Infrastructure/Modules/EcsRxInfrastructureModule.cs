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
using EcsRx.Systems.Handlers;

namespace EcsRx.Infrastructure.Modules
{
    public class EcsRxInfrastructureModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IIdPool, IdPool>();
            container.Bind<IEntityFactory, DefaultEntityFactory>();
            container.Bind<IEntityCollectionFactory, DefaultEntityCollectionFactory>();
            container.Bind<IEntityDatabase, EntityDatabase>();
            container.Bind<IObservableGroupFactory, DefaultObservableObservableGroupFactory>();
            container.Bind<IObservableGroupManager, ObservableGroupManager>();
            container.Bind<IConventionalSystemHandler, BasicEntitySystemHandler>();
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