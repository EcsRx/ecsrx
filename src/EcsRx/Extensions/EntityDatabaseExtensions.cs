using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class EntityDatabaseExtensions
    {
        /// <summary>
        /// Returns all entities from all the entity collections
        /// </summary>
        /// <param name="entityCollections">The entity collections to scan</param>
        /// <returns>All entities within the collections</returns>
        public static IEnumerable<IEntity> GetAllEntities(this IEnumerable<IEntityCollection> entityCollections)
        { return entityCollections.SelectMany(x => x); }
        
        /// <summary>
        /// Gets an enumerable collection of entities for you to iterate through,
        /// from the collection provided
        /// This is not cached and will always query the live data.
        /// </summary>
        /// <remarks>
        /// So in most cases an IObservableGroup is a better option to use for repeat queries as it internally
        /// will update a maintained list of entities without having to enumerate the entire collection/s.
        /// </remarks>
        /// <param name="group">The group to match entities on</param>
        /// <param name="collectionId">The optional collection name to use (defaults to null)</param>
        /// <returns>An enumerable to access the data inside the collection/s</returns>
        public static IEnumerable<IEntity> GetEntitiesFor(this IEntityDatabase database, IGroup group, int collectionId = EntityCollectionLookups.NoCollectionDefined)
        {
            if(group is EmptyGroup)
            { return Array.Empty<IEntity>(); }

            if (collectionId != EntityCollectionLookups.NoCollectionDefined)
            { return database[collectionId].MatchingGroup(group); }

            return database.Collections.GetAllEntities().MatchingGroup(group);
        }
        
        /// <summary>
        /// Gets an enumerable collection of entities for you to iterate through,
        /// it will by default search across ALL collections within the manager unless constrained.
        /// This is not cached and will always query the live data.
        /// </summary>
        /// <remarks>
        /// So in most cases an IObservableGroup is a better option to use for repeat queries as it internally
        /// will update a maintained list of entities without having to enumerate the entire collection/s.
        /// </remarks>
        /// <param name="group">The group to match entities on</param>
        /// <param name="collectionIds">The collection ids to scan</param>
        /// <returns>An enumerable to access the data inside the collection/s</returns>
        public static IEnumerable<IEntity> GetEntitiesFor(this IEntityDatabase database, IGroup group, params int[] collectionIds)
        {
            if(group is EmptyGroup)
            { return Array.Empty<IEntity>(); }

            if (collectionIds == null || collectionIds.Length == 0)
            { return database.Collections.GetAllEntities().MatchingGroup(group); }

            var matchingEntities = new List<IEntity>();
            foreach (var collectionId in collectionIds)
            {
                var results = database[collectionId].MatchingGroup(group);
                matchingEntities.AddRange(results);
            }

            return matchingEntities;
        }
        
        /// <summary>
        /// Gets an enumerable collection of entities for you to iterate through,
        /// it will by default search across ALL collections within the manager unless constrained.
        /// This is not cached and will always query the live data.
        /// </summary>
        /// <remarks>
        /// So in most cases an IObservableGroup is a better option to use for repeat queries as it internally
        /// will update a maintained list of entities without having to enumerate the entire collection/s.
        /// </remarks>
        /// <param name="lookupGroup">The lookup group to match entities on</param>
        /// <param name="collectionIds">The collection ids to scan</param>
        /// <returns>An enumerable to access the data inside the collection/s</returns>
        public static IEnumerable<IEntity> GetEntitiesFor(this IEntityDatabase database, LookupGroup lookupGroup, params int[] collectionIds)
        {
            if(lookupGroup.RequiredComponents.Length == 0 && lookupGroup.ExcludedComponents.Length  == 0)
            { return Array.Empty<IEntity>(); }

            if (collectionIds == null || collectionIds.Length == 0)
            { return database.Collections.GetAllEntities().MatchingGroup(lookupGroup); }

            var matchingEntities = new List<IEntity>();
            foreach (var collectionId in collectionIds)
            {
                var results = database[collectionId].MatchingGroup(lookupGroup);
                matchingEntities.AddRange(results);
            }

            return matchingEntities;
        }

        public static IEntityCollection GetCollectionFor(this IEntityDatabase entityDatabase, IEntity entity)
        { return GetCollectionFor(entityDatabase, entity.Id); }
        
        public static IEntityCollection GetCollectionFor(this IEntityDatabase entityDatabase, int entityId)
        { return entityDatabase.Collections.SingleOrDefault(x => x.ContainsEntity(entityId)); }

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

        public static IEntity GetEntity(this IEntityDatabase entityDatabase, int entityId)
        {
            var collection = GetCollectionFor(entityDatabase, entityId);
            return collection?.GetEntity(entityId);
        }
    }
}