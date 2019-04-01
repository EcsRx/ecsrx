using EcsRx.Components;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Accessors
{
    public interface IBatchAccessor
    {
        void Refresh();
    }

    public interface IBatchAccessor<T1, T2> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        Batch<T1, T2>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        Batch<T1, T2, T3>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3, T4> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3, T4, T5> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4, T5>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3, T4, T5, T6> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4, T5, T6>[] Batch { get; }
    }
    
}