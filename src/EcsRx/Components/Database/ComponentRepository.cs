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
            if(entityId <= Database.CurrentEntityBounds) { return; }
            Database.AccommodateMoreEntities(entityId + DefaultExpansionSize);
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

        public T Add<T>(int entityId, T component) where T : class, IComponent
        {
            var componentTypeId = ComponentLookup.GetComponentType<T>();
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