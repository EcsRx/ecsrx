using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Extensions;

namespace EcsRx.Groups.Batches
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
            RefreshBatches(ObservableGroup);
        }

        public void ListenToChanges()
        {
            ObservableGroup.OnEntityAdded.Subscribe(OnEntityAdded).AddTo(Subscriptions);
            ObservableGroup.OnEntityRemoved.Subscribe(OnEntityRemoved).AddTo(Subscriptions);
        }

        public void OnEntityAdded(IEntity entity)
        {
            var newIndex = Batches.Count;
            var newBatch = new T[newIndex + 1];
            ((T[])Batches).CopyTo(newBatch, 0);

            Batches = newBatch;
            
            foreach (var field in _fieldsToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);
                var components = GetComponentArray(field.FieldType, componentTypeId);
                var allocationIndex = entity.ComponentAllocations[componentTypeId];
                field.SetValue(Batches[newIndex], components[allocationIndex]);
            }

            foreach (var property in _propertiesToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);
                var components = GetComponentArray(property.PropertyType, componentTypeId);               
                var allocationIndex = entity.ComponentAllocations[componentTypeId];
                property.SetValue(Batches[newIndex], components[allocationIndex]);
            }
        }
        
        public void OnEntityRemoved(IEntity entity)
        {
            var currentIndex = 0;
            var newBatch = new T[Batches.Count - 1];

            for (var i = 0; i < Batches.Count; i++)
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