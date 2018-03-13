using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Extensions
{
    public static class EntityCollectionManagerExtensions
    {
        public static IEnumerable<IEntity> GetAllEntities(this IEnumerable<IEntityCollection> pools)
        { return pools.SelectMany(x => x); }

        public static IEntityCollection GetContainingPoolFor(this IEntityCollectionManager entityCollectionManager, IEntity entity)
        { return entityCollectionManager.Pools.SingleOrDefault(x => x.ContainsEntity(entity)); }

        public static void RemoveEntitiesContaining(this IEntityCollectionManager entityCollectionManager, params Type[] components)
        {
            foreach (var pool in entityCollectionManager.Pools)
            { pool.RemoveEntitiesContaining(components); }
        }

        public static void RemoveEntity(this IEntityCollectionManager entityCollectionManager, IEntity entity)
        {
            var containingPool = entityCollectionManager.GetContainingPoolFor(entity);
            containingPool.RemoveEntity(entity);
        }

        public static void RemoveEntities(this IEntityCollectionManager entityCollectionManager, Func<IEntity, bool> predicate)
        {
            var matchingEntities = entityCollectionManager.Pools.SelectMany(x => x).Where(predicate).ToArray();
            RemoveEntities(entityCollectionManager, matchingEntities);
        }

        public static void RemoveEntities(this IEntityCollectionManager entityCollectionManager, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { RemoveEntity(entityCollectionManager, entities[i]); }
        }

        public static void RemoveEntities(this IEntityCollectionManager entityCollectionManager, IEnumerable<IEntity> entities)
        {
            foreach(var entity in entities)
            { RemoveEntity(entityCollectionManager, entity);}
        }
    }
}