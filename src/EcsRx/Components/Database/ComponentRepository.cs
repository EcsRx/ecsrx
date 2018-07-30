using System;
using System.Collections.Generic;

namespace EcsRx.Components.Database
{
    public class ComponentRepository : IComponentRepository
    {
        public IComponentTypeLookup ComponentLookup { get; }
        public IComponentDatabase Database { get; }
        public int DefaultExpansionSize { get; }

        public ComponentRepository(IComponentTypeLookup componentLookup, IComponentDatabase database, int defaultExpansionSize = 100)
        {
            ComponentLookup = componentLookup;
            DefaultExpansionSize = defaultExpansionSize;
            Database = database;
        }

        public void ExpandDatabaseIfNeeded(int entityId)
        {
            if(entityId < Database.CurrentEntityBounds) { return; }
            var newSize = entityId + DefaultExpansionSize + 1;
            Database.AccommodateMoreEntities(newSize);
        }

        public Type[] GetTypesFor(params int[] componentTypeIds)
        { return ComponentLookup.GetComponentTypes(componentTypeIds); }
                
        public int[] GetTypesFor(params Type[] componentTypeIds)
        { return ComponentLookup.GetComponentTypes(componentTypeIds); }

        public int Add<T>(int entityId, T component) where T : class, IComponent
        {
            var underlyingType = component.GetType();
            var componentTypeId = ComponentLookup.GetComponentType(underlyingType);
            Database.Add(componentTypeId, entityId, component);
            return componentTypeId;
        }
        
        public IComponent Get(int entityId, int componentTypeId) => Database.Get(componentTypeId, entityId);        
        
        public IComponent Get(int entityId, Type componentType)
        {
            var componentTypeId = ComponentLookup.GetComponentType(componentType);
            return Get(entityId, componentTypeId);
        }

        public bool Has(int entityId, int componentTypeId) => Database.Has(componentTypeId, entityId);   
        
        public bool Has(int entityId, Type componentType)
        {
            var componentTypeId = ComponentLookup.GetComponentType(componentType);
            return Has(entityId, componentTypeId);
        }

        public IEnumerable<IComponent> GetAll(int entityId)
        { return Database.GetAll(entityId); }

        public void Remove(int entityId, int componentTypeId) => Database.Remove(componentTypeId, entityId);

        public void Remove(int entityId, Type componentType)
        {
            var componentTypeId = ComponentLookup.GetComponentType(componentType);
            Remove(entityId, componentTypeId);
        }
        
        public void RemoveAll(int entityId)
        { Database.RemoveAll(entityId); }
    }
}