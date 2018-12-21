using System.Collections.Generic;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Groups.Batches;
using EcsRx.Groups.Observable;
using EcsRx.Lookups;

namespace EcsRx.Collections
{
    public class BatchManager : IBatchManager
    {
        public BatchLookup BatchLookup { get; } = new BatchLookup();
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComponentDatabase ComponentDatabase { get; }

        public IReadOnlyList<IComponentBatches> Batches => BatchLookup;

        public BatchManager(IComponentTypeLookup componentTypeLookup, IComponentDatabase componentDatabase)
        {
            ComponentTypeLookup = componentTypeLookup;
            ComponentDatabase = componentDatabase;
        }

        public IObservableComponentBatches<T> GetBatch<T>(IObservableGroup group) where T : IBatchDescriptor
        {
            var batchType = typeof(T);
            if (BatchLookup.Contains(batchType))
            { return (IObservableComponentBatches<T>)BatchLookup[typeof(T)]; }

            var newBatch = new ObservableComponentBatches<T>(ComponentTypeLookup, ComponentDatabase, group);
            BatchLookup.Add(newBatch);
            return newBatch;
        }

        public IManualComponentBatches<T> GetBatch<T>() where T : IBatchDescriptor
        {
            var batchType = typeof(T);
            if (BatchLookup.Contains(batchType))
            { return (IManualComponentBatches<T>)BatchLookup[typeof(T)]; }

            var newBatch = new ManualComponentBatches<T>(ComponentTypeLookup, ComponentDatabase);
            BatchLookup.Add(newBatch);
            return newBatch;
        }
    }
}