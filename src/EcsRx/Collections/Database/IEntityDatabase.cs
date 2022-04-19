using System;
using System.Collections.Generic;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Collections.Database
{
    public static class EntityCollectionLookups
    {
        public const int NoCollectionDefined = -1;
        public const int DefaultCollectionId = 0;
    }
    
    /// <summary>
    /// This acts as the database to store all entities, rather than containing all entities directly
    /// within itself, it partitions them into collections which can contain differing amounts of entities.
    /// </summary>
    public interface IEntityDatabase : INotifyingEntityCollection, IDisposable
    {
        /// <summary>
        /// All the entity collections that the manager contains
        /// </summary>
        IReadOnlyList<IEntityCollection> Collections { get; }
        
        /// <summary>
        /// Fired when a collection has been added
        /// </summary>
        IObservable<IEntityCollection> CollectionAdded { get; }
        
        /// <summary>
        /// Fired when a collection has been removed
        /// </summary>
        IObservable<IEntityCollection> CollectionRemoved { get; }
        
        /// <summary>
        /// Creates a new collection within the database
        /// </summary>
        /// <remarks>
        /// This is primarily useful for when you want to isolate certain entities, such as short lived ones which would
        /// be constantly being destroyed and recreated, like bullets etc. In most cases you will probably not need more than 1.
        /// </remarks>
        /// <param name="id">The name to give the collection</param>
        /// <returns>A newly created collection with that name</returns>
        IEntityCollection CreateCollection(int id);
        
        /// <summary>
        /// Adds an existing collection within the database
        /// </summary>
        /// <remarks>
        /// This is mainly used for when you have persisted a collection and want to re-load it
        /// </remarks>
        /// <param name="collection">The collection to add</param>
        void AddCollection(IEntityCollection collection);
        
        /// <summary>
        /// Gets a collection by id from within the manager, if no id is provided the default pool is returned
        /// </summary>
        /// <param name="id">The optional id of collection to return</param>
        /// <returns>The located collection</returns>
        /// <remarks>This is a safe Get so it will return back a null if no collection can be found</remarks>
        IEntityCollection GetCollection(int id = EntityCollectionLookups.DefaultCollectionId);
        
        /// <summary>
        /// Provides a mechanism to directly get the collection from the underlying store, however will throw if doesnt exist
        /// </summary>
        /// <param name="id">The id of the collection</param>
        /// <remarks>This is more performant but unsafe way to access the collections directly by id</remarks>
        IEntityCollection this[int id] { get; }
        
        /// <summary>
        /// Removes a collection from the manager
        /// </summary>
        /// <param name="id">The collection to remove</param>
        /// <param name="disposeEntities">if the entities should all be disposed too</param>
        void RemoveCollection(int id, bool disposeEntities = true);
    }
}