using EcsRx.Components;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Accessors
{
    public interface IReferenceBatchAccessor : IBatchAccessor { }
       
    public interface IReferenceBatchAccessor<T1, T2> : IReferenceBatchAccessor
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        ReferenceBatch<T1, T2>[] Batch { get; }
    }
    
    public interface IReferenceBatchAccessor<T1, T2, T3> : IReferenceBatchAccessor
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3>[] Batch { get; }
    }
    
    public interface IReferenceBatchAccessor<T1, T2, T3, T4> : IReferenceBatchAccessor
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3, T4>[] Batch { get; }
    }
    
    public interface IReferenceBatchAccessor<T1, T2, T3, T4, T5> : IReferenceBatchAccessor
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3, T4, T5>[] Batch { get; }
    }
    
    public interface IReferenceBatchAccessor<T1, T2, T3, T4, T5, T6> : IReferenceBatchAccessor
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3, T4, T5, T6>[] Batch { get; }
    }
}