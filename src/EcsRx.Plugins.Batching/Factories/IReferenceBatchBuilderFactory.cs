using EcsRx.Components;
using EcsRx.Plugins.Batching.Builders;

namespace EcsRx.Plugins.Batching.Factories
{
    public interface IReferenceBatchBuilderFactory
    {
        IReferenceBatchBuilder<T1, T2> Create<T1, T2>()
            where T1 : class, IComponent
            where T2 : class, IComponent;

        IReferenceBatchBuilder<T1, T2, T3> Create<T1, T2, T3>()
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent;

        IReferenceBatchBuilder<T1, T2, T3, T4> Create<T1, T2, T3, T4>()
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent;
        
        IReferenceBatchBuilder<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>()
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent
            where T5 : class, IComponent;
        
        IReferenceBatchBuilder<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>()
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent
            where T5 : class, IComponent
            where T6 : class, IComponent;
    }
}