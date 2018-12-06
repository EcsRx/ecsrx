using System;
using System.Collections.Generic;

namespace EcsRx.Components.Database
{
    public interface IComponentDatabase
    {
        int CurrentEntityBounds { get; }
        void AccommodateMoreEntities(int newMaxSize);
        
        T Get<T>(int componentTypeId, int entityId) where T : IComponent;
        void Set<T>(int componentTypeId, int entityId, T component) where T : IComponent;
        bool Has(int componentTypeId, int entityId);
        void Remove(int componentTypeId, int entityId);

        IReadOnlyList<T> GetComponents<T>(int componentTypeId) where T : IComponent;
        IEnumerable<IComponent> GetAll(int entityId);

        void RemoveAll(int entityId);
    }
}