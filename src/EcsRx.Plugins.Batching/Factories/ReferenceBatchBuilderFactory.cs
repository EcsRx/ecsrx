using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Plugins.Batching.Builders;

namespace EcsRx.Plugins.Batching.Factories
{
    public class ReferenceBatchBuilderFactory : IReferenceBatchBuilderFactory
    {
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public ReferenceBatchBuilderFactory(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
        }

        public IReferenceBatchBuilder<T1, T2> Create<T1, T2>() 
            where T1 : class, IComponent 
            where T2 : class, IComponent
        { return new ReferenceBatchBuilder<T1, T2>(ComponentDatabase, ComponentTypeLookup); }

        public IReferenceBatchBuilder<T1, T2, T3> Create<T1, T2, T3>() 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent
        {
            return new ReferenceBatchBuilder<T1, T2, T3>(ComponentDatabase, ComponentTypeLookup);
        }

        public IReferenceBatchBuilder<T1, T2, T3, T4> Create<T1, T2, T3, T4>() 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent
        {
            return new ReferenceBatchBuilder<T1, T2, T3, T4>(ComponentDatabase, ComponentTypeLookup);
        }

        public IReferenceBatchBuilder<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>() 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent 
            where T5 : class, IComponent
        {
            return new ReferenceBatchBuilder<T1, T2, T3, T4, T5>(ComponentDatabase, ComponentTypeLookup);
        }

        public IReferenceBatchBuilder<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>() 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent 
            where T5 : class, IComponent 
            where T6 : class, IComponent
        {
            return new ReferenceBatchBuilder<T1, T2, T3, T4, T5, T6>(ComponentDatabase, ComponentTypeLookup);
        }
    }
}