using System;
using System.Collections.Generic;
using EcsRx.Components.Lookups;

namespace EcsRx.Components.Database
{
    public class ComponentRepository : IComponentRepository
    {
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IComponentDatabase Database { get; }
        public int DefaultExpansionSize { get; }

        public ComponentRepository(IComponentTypeLookup componentLookup, IComponentDatabase database, int defaultExpansionSize = 5000)
        {
            ComponentTypeLookup = componentLookup;
            DefaultExpansionSize = defaultExpansionSize;
            Database = database;
        }

        public void ExpandDatabaseIfNeeded(int entityId)
        {
            if(entityId < Database.CurrentEntityBounds) { return; }
            var newSize = entityId + DefaultExpansionSize + 1;
            Database.AccommodateMoreEntities(newSize);
        }

        public int Add<T>(int entityId, T component) where T : IComponent
        {
            var underlyingType = component.GetType();
            var componentTypeId = ComponentTypeLookup.GetComponentType(underlyingType);
            
            #if DEBUG
            if(Database.Has(componentTypeId, entityId))
            { throw new Exception("DEBUG ONLY ERROR: Component already exists on entity"); }
            #endif
            
            Database.Set(componentTypeId, entityId, component);
            return componentTypeId;
        }

        public T Create<T>(int entityId, int componentTypeId) where T : IComponent, new()
        {
            #if DEBUG
            if(Database.Has(componentTypeId, entityId))
            { throw new Exception("DEBUG ONLY ERROR: Component already exists on entity"); }
            #endif
            
            var defaultComponent = ComponentTypeLookup.CreateDefault<T>();
            Database.Set(componentTypeId, entityId, defaultComponent);
            return defaultComponent;
        }       

        public T Get<T>(int entityId, int componentTypeId) where T : IComponent
        { return Database.Get<T>(componentTypeId, entityId); }

        public bool Has(int entityId, int componentTypeId) => Database.Has(componentTypeId, entityId);          
        public IEnumerable<IComponent> GetAll(int entityId) => Database.GetAll(entityId);
        public void Remove(int entityId, int componentTypeId) => Database.Remove(componentTypeId, entityId);
        public void RemoveAll(int entityId) => Database.RemoveAll(entityId);
    }
}