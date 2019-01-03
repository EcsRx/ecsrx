using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;

namespace EcsRx.Groups.Batches
{
    public class ManualComponentBatches<T> : IManualComponentBatches<T> where T : IBatchDescriptor
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public T[] Batches { get; protected set; }

        protected MethodInfo _getComponentMethod;
        protected FieldInfo[] _fieldsToSet;
        protected PropertyInfo[] _propertiesToSet;
        protected PropertyInfo _entityIdSetter;

        public ManualComponentBatches(IComponentTypeLookup componentTypeLookup, IComponentDatabase componentDatabase)
        {
            ComponentTypeLookup = componentTypeLookup;
            ComponentDatabase = componentDatabase;
            AnalyzeBatch();
        }

        public void InitializeBatches(IReadOnlyList<IEntity> entities)
        {
            if(Batches == null || Batches.Length != entities.Count)
            { Batches = new T[entities.Count]; }
           
            foreach (var field in _fieldsToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);
                var components = GetComponent(field.FieldType, componentTypeId);

                for (var i = 0; i < entities.Count; i++)
                {
                    ref var batchItem = ref Batches[i];
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = components[allocationIndex];
                    field.SetValueDirect(__makeref(batchItem), component);
                }
            }

            foreach (var property in _propertiesToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);
                var components = GetComponent(property.PropertyType, componentTypeId);

                for (var i = 0; i < entities.Count; i++)
                {
                    ref var batchItem = ref Batches[i];
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = components[allocationIndex];
                    property.SetValue(batchItem, component);
                }
            }
            
            for (var i = 0; i < entities.Count; i++)
            {
                _entityIdSetter.SetValue(Batches[i], entities[i].Id, null);
            }
        }

        public void AnalyzeBatch()
        {
            var databaseType = ComponentDatabase.GetType();
            _getComponentMethod = databaseType.GetMethod("GetComponents");
            
            var batchType = typeof(T);
            _fieldsToSet = batchType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            _propertiesToSet = batchType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name != "EntityId").ToArray();

            _entityIdSetter = batchType.GetProperty("EntityId", BindingFlags.Public | BindingFlags.Instance);
        }
       
        public IList GetComponent(Type type, int componentTypeId)
        {
            var genericMethod = _getComponentMethod.MakeGenericMethod(type);
            return (IList)genericMethod.Invoke(ComponentDatabase, new object[]{componentTypeId});
        }

        public void RefreshBatches(IReadOnlyList<IEntity> entities)
        {
            if(Batches == null || Batches.Length != entities.Count)
            { InitializeBatches(entities); }
            
            foreach (var field in _fieldsToSet)
            {
                if (!field.FieldType.IsValueType) { continue; }
                
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);
                var components = GetComponent(field.FieldType, componentTypeId);

                for (var i = 0; i < entities.Count; i++)
                {
                    ref var batchItem = ref Batches[i];
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = components[allocationIndex];
                    field.SetValueDirect(__makeref(batchItem), component);
                }
            }

            foreach (var property in _propertiesToSet)
            {
                if (!property.PropertyType.IsValueType) { continue; }
                
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);
                var components = GetComponent(property.PropertyType, componentTypeId);

                for (var i = 0; i < entities.Count; i++)
                {
                    ref var batchItem = ref Batches[i];
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = components[allocationIndex];
                    property.SetValue(batchItem, component);
                }
            }
        }
    }

    
    /*
    public class ManualComponentBatches<T> : IManualComponentBatches<T> where T : IBatchDescriptor
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComponentDatabase ComponentDatabase { get; }
        public T[] Batches { get; protected set; }

        protected MethodInfo _getComponentMethod;
        protected FieldInfo[] _fieldsToSet;
        protected PropertyInfo[] _propertiesToSet;

        public ManualComponentBatches(IComponentTypeLookup componentTypeLookup, IComponentDatabase componentDatabase)
        {
            ComponentTypeLookup = componentTypeLookup;
            ComponentDatabase = componentDatabase;
            AnalyzeBatch();
        }

        public void AnalyzeBatch()
        {
            var databaseType = ComponentDatabase.GetType();
            _getComponentMethod = databaseType.GetMethod("Get");
            
            var batchType = typeof(T);
            _fieldsToSet = batchType.GetFields(BindingFlags.Public|BindingFlags.Instance);
            _propertiesToSet = batchType.GetProperties(BindingFlags.Public|BindingFlags.Instance).Where(x => x.Name != "EntityId").ToArray();
        }
       
        public object GetComponent(Type type, int componentTypeId, int allocationId)
        {
            var genericMethod = _getComponentMethod.MakeGenericMethod(type);
            return genericMethod.Invoke(ComponentDatabase, new object[]{componentTypeId, allocationId});
        }

        public void InitializeBatches(IReadOnlyList<IEntity> entities)
        {
            Batches = new T[entities.Count];
           
            foreach (var field in _fieldsToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);

                for (var i = 0; i < entities.Count; i++)
                {
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = GetComponent(field.FieldType, componentTypeId, allocationIndex);
                    field.SetValueDirect(__makeref(Batches[i]), component);
                }
            }

            foreach (var property in _propertiesToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);

                for (var i = 0; i < entities.Count; i++)
                {
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = GetComponent(property.PropertyType, componentTypeId, allocationIndex);
                    property.SetValue(Batches[i], component);
                }
            }
        }

        public void RefreshBatches(IReadOnlyList<IEntity> entities)
        {
            foreach (var field in _fieldsToSet)
            {
                if (!field.FieldType.IsValueType) { continue; }
                
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);

                for (var i = 0; i < Batches.Length; i++)
                {
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = GetComponent(field.FieldType, componentTypeId, allocationIndex);
                    field.SetValueDirect(__makeref(Batches[i]), component);
                }
            }

            foreach (var property in _propertiesToSet)
            {
                if (!property.PropertyType.IsValueType) { continue; }
                
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);

                for (var i = 0; i < Batches.Length; i++)
                {
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = GetComponent(property.PropertyType, componentTypeId, allocationIndex);
                    property.SetValue(Batches[i], component);
                }
            }
        }
    }
    */
}