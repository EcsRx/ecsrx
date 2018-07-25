using System;
using System.Collections.Generic;

namespace EcsRx.Components.Database
{
    public interface IComponentRepository
    {
        void ExpandDatabaseIfNeeded(int entityId);
        
        IComponent Get(int entityId, Type componentType);
        IEnumerable<IComponent> GetAll(int entityId);
        T Add<T>(int entityId, T component) where T : class, IComponent;
        bool Has(int entityId, Type componentType);
        void Remove(int entityId, Type componentType);
        void RemoveAll(int entityId);
    }
}