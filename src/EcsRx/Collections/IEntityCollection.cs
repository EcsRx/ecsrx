using System;
using System.Collections.Generic;
using EcsRx.Blueprints;
using EcsRx.Entities;

namespace EcsRx.Collections
{
    public interface IEntityCollection : IEnumerable<IEntity>
    {
        string Name { get; }
        
        IEntity CreateEntity(IBlueprint blueprint = null);
        void AddEntity(IEntity entity);
        
        IEntity GetEntity(Guid id);
        bool ContainsEntity(Guid id);
        void RemoveEntity(Guid id);
    }
}
