using System;
using EcsRx.Components.Database;
using EcsRx.Pools;

namespace EcsRx.Entities
{
    public class DefaultEntityFactory : IEntityFactory
    {
        public IIdPool IdPool { get; }
        public IComponentRepository ComponentRepository { get; }

        public DefaultEntityFactory(IIdPool idPool, IComponentRepository componentRepository)
        {
            IdPool = idPool;
            ComponentRepository = componentRepository;
        }

        public int GetId(int? id = null)
        {
            if(!id.HasValue)
            { return IdPool.AllocateInstance(); }

            IdPool.AllocateSpecificId(id.Value);
            return id.Value;
        }
        
        public IEntity Create(int? id = null)
        {
            if(id.HasValue && id.Value == 0)
            { throw new ArgumentException("id must be null or > 0"); }
            
            var usedId = GetId(id);
            return new Entity(usedId, ComponentRepository);
        }

        public void Destroy(int entityId)
        { IdPool.ReleaseInstance(entityId); }
    }
}