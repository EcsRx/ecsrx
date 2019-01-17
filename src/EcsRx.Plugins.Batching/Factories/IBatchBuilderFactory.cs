using EcsRx.Components;
using EcsRx.Plugins.Batching.Builders;

namespace EcsRx.Plugins.Batching.Factories
{
    public interface IBatchBuilderFactory
    {
        IBatchBuilder<T1, T2> Create<T1, T2>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;

        IBatchBuilder<T1, T2, T3> Create<T1, T2, T3>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;

        IBatchBuilder<T1, T2, T3, T4> Create<T1, T2, T3, T4>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        
        IBatchBuilder<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        
        IBatchBuilder<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;
    }
}