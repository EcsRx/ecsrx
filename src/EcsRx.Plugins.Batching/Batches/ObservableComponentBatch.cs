using System;
using System.Collections.Generic;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Extensions;

namespace EcsRx.Plugins.Batching.Batches
{
    public class ObservableComponentBatches<T> : ManualComponentBatches<T>, IObservableComponentBatches<T> where T : IBatchDescriptor
    {
        public IObservableGroup ObservableGroup { get; }
        
        public List<IDisposable> Subscriptions { get; }

        public ObservableComponentBatches(IComponentTypeLookup componentTypeLookup, IComponentDatabase componentDatabase, IObservableGroup observableGroup) 
            : base(componentTypeLookup, componentDatabase)
        {
            ObservableGroup = observableGroup;
            Subscriptions = new List<IDisposable>();
            ListenToChanges();
            InitializeBatches(ObservableGroup);
        }

        public void ListenToChanges()
        {
            ObservableGroup.OnEntityAdded.Subscribe(OnEntityAdded).AddTo(Subscriptions);
            ObservableGroup.OnEntityRemoved.Subscribe(OnEntityRemoved).AddTo(Subscriptions);
        }

        public void OnEntityAdded(IEntity entity)
        {
            var newIndex = Batches.Length;
            var newBatch = new T[newIndex + 1];
            Batches.CopyTo(newBatch, 0);

            Batches = newBatch;
            
            /*
            foreach (var field in _fieldsToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);
                var allocationIndex = entity.ComponentAllocations[componentTypeId];
                var component = GetComponents(field.FieldType, componentTypeId, allocationIndex);
                
                field.SetValue(Batches[newIndex], component);
            }

            foreach (var property in _propertiesToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);
                var allocationIndex = entity.ComponentAllocations[componentTypeId];
                var component = GetComponent(property.PropertyType, componentTypeId, allocationIndex);               

                property.SetValue(Batches[newIndex], component);
            }*/
        }
        
        public void OnEntityRemoved(IEntity entity)
        {
            var currentIndex = 0;
            var newBatch = new T[Batches.Length - 1];

            for (var i = 0; i < Batches.Length; i++)
            {
                if (Batches[i].EntityId == entity.Id) { continue; }
                newBatch[currentIndex++] = Batches[i];
            }

            Batches = newBatch;
        }

        public void Dispose()
        { Subscriptions.DisposeAll(); }
    }
}