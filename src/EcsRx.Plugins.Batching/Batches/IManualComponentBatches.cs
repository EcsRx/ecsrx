using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Plugins.Batching.Batches
{
    public interface IManualComponentBatches<out T> : IComponentBatches<T> where T : IBatchDescriptor
    {
        void InitializeBatches(IReadOnlyList<IEntity> entities);
        void RefreshBatches(IReadOnlyList<IEntity> entities);
    }
}