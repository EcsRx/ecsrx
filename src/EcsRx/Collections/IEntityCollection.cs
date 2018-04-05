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
        bool ContainsEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
    }
}
