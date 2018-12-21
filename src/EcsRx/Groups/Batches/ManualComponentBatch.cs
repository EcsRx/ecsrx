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
        public IReadOnlyList<T> Batches { get; protected set; }

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
            _getComponentMethod = databaseType.GetMethod("GetComponents");
            
            var batchType = typeof(T);
            _fieldsToSet = batchType.GetFields(BindingFlags.Public|BindingFlags.Instance);
            _propertiesToSet = batchType.GetProperties(BindingFlags.Public|BindingFlags.Instance).Where(x => x.Name != "EntityId").ToArray();
        }
       
        public IList GetComponentArray(Type type, int componentTypeId)
        {
            var genericMethod = _getComponentMethod.MakeGenericMethod(type);
            return (IList)genericMethod.Invoke(ComponentDatabase, new object[]{componentTypeId});
        }

        public void RefreshBatches(IReadOnlyList<IEntity> entities)
        {
            Batches = new T[entities.Count];
            
            foreach (var field in _fieldsToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(field.FieldType);
                var components = GetComponentArray(field.FieldType, componentTypeId);

                for (var i = 0; i < entities.Count; i++)
                {
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    var component = components[allocationIndex];
                    var batchItem = Batches[i];
                    field.SetValue(batchItem, component);
                }
            }

            foreach (var property in _propertiesToSet)
            {
                var componentTypeId = ComponentTypeLookup.GetComponentType(property.PropertyType);
                var components = GetComponentArray(property.PropertyType, componentTypeId);

                for (var i = 0; i < entities.Count; i++)
                {
                    var allocationIndex = entities[i].ComponentAllocations[componentTypeId];
                    property.SetValue(Batches[i], components[allocationIndex]);
                }
            }
        }
    }
}