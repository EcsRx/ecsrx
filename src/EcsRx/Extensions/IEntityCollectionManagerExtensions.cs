using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Extensions
{
    public static class IEntityCollectionManagerExtensions
    {
        public static IEnumerable<IEntity> GetAllEntities(this IEnumerable<IEntityCollection> pools)
        { return pools.SelectMany(x => x); }

        public static IEntityCollection GetCollectionFor(this IEntityCollectionManager entityCollectionManager, IEntity entity)
        { return entityCollectionManager.Collections.SingleOrDefault(x => x.ContainsEntity(entity.Id)); }

        public static void RemoveEntitiesContaining(this IEntityCollectionManager entityCollectionManager, params Type[] components)
        {
            foreach (var pool in entityCollectionManager.Collections)
            { pool.RemoveEntitiesContaining(components); }
        }

        public static void RemoveEntity(this IEntityCollectionManager entityCollectionManager, IEntity entity)
        {
            var containingPool = entityCollectionManager.GetCollectionFor(entity);
            containingPool.RemoveEntity(entity.Id);
        }

        public static void RemoveEntities(this IEntityCollectionManager entityCollectionManager, Func<IEntity, bool> predicate)
        {
            var matchingEntities = entityCollectionManager.Collections.SelectMany(x => x).Where(predicate).ToArray();
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

        public static IEntity GetEntity(this IEntityCollectionManager entityCollectionManager, int id)
        {
            return entityCollectionManager.Collections
                .Select(collection => collection.GetEntity(id))
                .FirstOrDefault(possibleEntity => possibleEntity != null);
        }
    }
}