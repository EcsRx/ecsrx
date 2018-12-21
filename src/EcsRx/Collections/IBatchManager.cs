using System.Collections.Generic;
using EcsRx.Groups.Batches;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    public interface IBatchManager
    {
        IReadOnlyList<IComponentBatches> Batches { get; }
        IObservableComponentBatches<T> GetBatch<T>(IObservableGroup group) where T : IBatchDescriptor;
        IManualComponentBatches<T> GetBatch<T>() where T : IBatchDescriptor;
    }
}