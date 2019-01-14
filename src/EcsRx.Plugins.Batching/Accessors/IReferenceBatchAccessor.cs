using EcsRx.Components;
using EcsRx.Plugins.Batching.Descriptors;

namespace EcsRx.Plugins.Batching.Accessors
{
    public interface IReferenceBatchAccessor : IBatchAccessor { }
    
    public class ReferenceBatchAccessor<T1> : IReferenceBatchAccessor
        where T1 : class, IComponent
    {
        ReferenceBatch<T1>[] Batch { get; }
    }
}