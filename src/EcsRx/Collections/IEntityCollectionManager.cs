using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    /// <summary>
    /// This acts as the database to store all entities, rather than containing all entities directly
    /// within itself, it partitions them into collections which can contain differing amounts of entities.
    /// </summary>
    public interface IEntityCollectionManager
    {
        /// <summary>
        /// All the entity collections that the manager contains
        /// </summary>
        IEnumerable<IEntityCollection> Collections { get; }

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
        /// <param name="collectionName">The optional collection name to use (defaults to null)</param>
        /// <returns>An enumerable to access the data inside the collection/s</returns>
        IEnumerable<IEntity> GetEntitiesFor(IGroup group, string collectionName = null);
        
        /// <summary>
        /// Gets an ObservableGroup which will observe the given group and maintain a collection of
        /// entities which are applicable. This is the preferred way to access entities inside collections.
        /// </summary>
        /// <remarks>
        /// It is worth noting that IObservableGroup instances are cached within the manager, so if there is
        /// a request for an observable group targetting the same underlying components (not the IGroup instance, but
        /// the actual components the group cares about) it will return the existing group, if one does not exist
        /// it is created.
        /// </remarks>
        /// <param name="group">The group to match entities on</param>
        /// <param name="collectionName">The optional collection name to use (defaults to null)</param>
        /// <returns>An IObservableGroup monitoring the group passed in</returns>
        IObservableGroup GetObservableGroup(IGroup group, string collectionName = null);

        /// <summary>
        /// Creates a new collection within the manager
        /// </summary>
        /// <remarks>
        /// This is primarily useful for when you want to isolate certain entities, such as short lived ones which would
        /// be constantly being destroyed and recreated, like bullets etc. In most cases you will probably not need more than 1.
        /// </remarks>
        /// <param name="name">The name to give the collection</param>
        /// <returns>A newly created collection with that name</returns>
        IEntityCollection CreateCollection(string name);
        
        /// <summary>
        /// Gets a collection by name from within the manager, if no name is provided the default pool is returned
        /// </summary>
        /// <param name="name">The optional name of collection to return</param>
        /// <returns>The located collection</returns>
        IEntityCollection GetCollection(string name = null);
        
        /// <summary>
        /// Removes a collection from the manager
        /// </summary>
        /// <param name="name">The collection to remove</param>
        void RemoveCollection(string name, bool disposeEntities = true);
    }
}