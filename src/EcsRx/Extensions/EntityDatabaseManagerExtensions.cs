using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Entities;

namespace EcsRx.Extensions
{
    public static class EntityDatabaseManagerExtensions
    {
        public static IEnumerable<IEntity> GetAllEntities(this IEnumerable<IEntityCollection> pools)
        { return pools.SelectMany(x => x); }

        public static IEntityCollection GetCollectionFor(this IEntityDatabase entityDatabase, IEntity entity)
        { return entityDatabase.Collections.SingleOrDefault(x => x.ContainsEntity(entity.Id)); }

        public static void RemoveEntitiesContaining(this IEntityDatabase entityDatabase, params Type[] components)
        {
            foreach (var pool in entityDatabase.Collections)
            { pool.RemoveEntitiesContaining(components); }
        }

        public static void RemoveEntity(this IEntityDatabase entityDatabase, IEntity entity)
        {
            var containingPool = entityDatabase.GetCollectionFor(entity);
            containingPool.RemoveEntity(entity.Id);
        }

        public static void RemoveEntities(this IEntityDatabase entityDatabase, Func<IEntity, bool> predicate)
        {
            var matchingEntities = entityDatabase.Collections.SelectMany(x => x).Where(predicate).ToArray();
            RemoveEntities(entityDatabase, matchingEntities);
        }

        public static void RemoveEntities(this IEntityDatabase entityDatabase, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { RemoveEntity(entityDatabase, entities[i]); }
        }

        public static void RemoveEntities(this IEntityDatabase entityDatabase, IEnumerable<IEntity> entities)
        {
            foreach(var entity in entities)
            { RemoveEntity(entityDatabase, entity);}
        }

        public static IEntity GetEntity(this IEntityDatabase entityDatabase, int id)
        {
            return entityDatabase.Collections
                .Select(collection => collection.GetEntity(id))
                .FirstOrDefault(possibleEntity => possibleEntity != null);
        }
    }
}