using System.Collections.Generic;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Collections
{
    public interface IBatchManager
    {
        IReadOnlyList<IComponentBatches> Batches { get; }
        IObservableComponentBatches<T> GetBatch<T>(IObservableGroup group) where T : IBatchDescriptor;
        IManualComponentBatches<T> GetBatch<T>() where T : IBatchDescriptor;
    }
}