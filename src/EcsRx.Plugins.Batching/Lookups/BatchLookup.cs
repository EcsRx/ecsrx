using System;
using System.Collections.ObjectModel;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Lookups
{
    public class BatchLookup : KeyedCollection<Type, IComponentBatches>
    {
        protected override Type GetKeyForItem(IComponentBatches item) => item.GetType();

        public IComponentBatches GetByIndex(int index) => Items[index];
    }
}