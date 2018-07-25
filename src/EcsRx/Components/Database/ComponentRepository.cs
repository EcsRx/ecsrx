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
       
        public IComponent Get(int entityId, Type componentType)
        {
            var componentTypeId = ComponentLookup.GetComponentType(componentType);
            return Database.Get(componentTypeId, entityId);
        }
        
        public bool Has(int entityId, Type componentType)
        {
            var componentTypeId = ComponentLookup.GetComponentType(componentType);
            return Database.Has(componentTypeId, entityId);
        }
        
        public IEnumerable<IComponent> GetAll(int entityId)
        { return Database.GetAll(entityId); }

        public void RemoveAll(int entityId)
        { Database.RemoveAll(entityId); }

        public T Add<T>(int entityId, T component) where T : class, IComponent
        {
            var underlyingType = component.GetType();
            var componentTypeId = ComponentLookup.GetComponentType(underlyingType);
            Database.Add(componentTypeId, entityId, component);
            return component;
        }

        public void Remove(int entityId, Type componentType)
        {
            var componentTypeId = ComponentLookup.GetComponentType(componentType);
            Database.Remove(componentTypeId, entityId);
        }
    }
}