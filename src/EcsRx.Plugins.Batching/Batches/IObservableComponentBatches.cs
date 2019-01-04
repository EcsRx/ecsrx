using System;
using EcsRx.Groups.Observable;

namespace EcsRx.Plugins.Batching.Batches
{
    public interface IObservableComponentBatches<out T> : IManualComponentBatches<T>, IDisposable  where T : IBatchDescriptor
    {
        IObservableGroup ObservableGroup { get; }
    }
}