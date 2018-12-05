using System;
using System.Collections.Generic;

namespace EcsRx.Components.Database
{
    public interface IComponentDatabase
    {
        int CurrentEntityBounds { get; }
        void AccommodateMoreEntities(int newMaxSize);
        
        IComponent Get(int componentTypeId, int entityId);
        T Get<T>(int componentTypeId, int entityId);
        
        bool Has(int componentTypeId, int entityId);
        void Add(int componentTypeId, int entityId, IComponent component);
        void Remove(int componentTypeId, int entityId);

        IReadOnlyList<IComponent> GetComponents(int componentTypeId);
        IReadOnlyList<T> GetComponentStructs<T>(int componentTypeId) where T : struct;

        IEnumerable<IComponent> GetAll(int entityId);
        void RemoveAll(int entityId);
    }

}