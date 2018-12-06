using System;
using System.Collections.Generic;
using EcsRx.Components.Lookups;

namespace EcsRx.Components.Database
{   
    public interface IComponentRepository
    {
        void ExpandDatabaseIfNeeded(int entityId);
        
        IComponentTypeLookup ComponentTypeLookup { get; }
        
        T Get<T>(int entityId, int componentTypeId) where T : IComponent;
        IEnumerable<IComponent> GetAll(int entityId);

        int Add<T>(int entityId, T component) where T : IComponent;
        T Create<T>(int entityId, int componentTypeId) where T : IComponent, new();
        
        bool Has(int entityId, int componentTypeId);
        
        void Remove(int entityId, int componentTypeId);
        void RemoveAll(int entityId);
    }
}