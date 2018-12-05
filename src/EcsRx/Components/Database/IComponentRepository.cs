using System;
using System.Collections.Generic;

namespace EcsRx.Components.Database
{   
    public interface IComponentRepository
    {
        void ExpandDatabaseIfNeeded(int entityId);
        Type[] GetTypesFor(params int[] componentTypeIds);
        int[] GetTypesFor(params Type[] componentTypeIds);
        
        IComponent Get(int entityId, Type componentType);
        IComponent Get(int entityId, int componentTypeId);
        T Get<T>(int entityId, int componentTypeId);
        IEnumerable<IComponent> GetAll(int entityId);

        int Add(int entityId, IComponent component);
        T Create<T>(int entityId, int componentTypeId) where T : struct;
        
        bool Has(int entityId, Type componentType);
        bool Has(int entityId, int componentTypeId);
        
        void Remove(int entityId, Type componentType);
        void Remove(int entityId, int componentTypeId);
        void RemoveAll(int entityId);
    }
}