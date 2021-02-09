using System.Collections.Generic;
using EcsRx.Blueprints;
using EcsRx.Entities;

namespace EcsRx.Collections.Entity
{
    /// <summary>
    /// The entity collection is a container for entities, it can be seen as a Repository of sorts
    /// as it allows for CRUD based operations and querying (through extensions)
    /// </summary>
    public interface IEntityCollection : IReadOnlyList<IEntity>, INotifyingEntityCollection
    {
        /// <summary>
        /// Name of the collection
        /// </summary>
        int Id { get; }

        /// <summary>
        /// This will create and return a new entity.
        /// If required you can pass in a blueprint which the created entity will conform to
        /// </summary>
        /// <param name="blueprint">Optional blueprint to use for the entity (defaults to null)</param>
        /// <param name="id">Id to use for the entity (defaults to null, meaning it'll automatically get the next available id)</param>
        /// <returns></returns>
        IEntity CreateEntity(IBlueprint blueprint = null, int? id = null);
        
        /// <summary>
        /// This will add an existing entity into the group, it is mainly used for pre-made
        /// entities which have been created from persisted data/serializers etc.
        /// Should be used with care as you should only have an entity in one collection.
        /// </summary>
        /// <param name="entity">Entity to add to the collection</param>
        void AddEntity(IEntity entity);
        
        /// <summary>
        /// Gets the entity from the collection, this will return the IEntity or null
        /// </summary>
        /// <param name="id">The Id of the entity you want to locate</param>
        /// <returns>The entity that has been located or null if one could not be found</returns>
        IEntity GetEntity(int id);
        
        /// <summary>
        /// Checks if the collection contains a given entity
        /// </summary>
        /// <param name="id">The Id of the entity you want to locate</param>
        /// <returns>true if it finds the entity, false if it cannot</returns>
        bool ContainsEntity(int id);
        
        /// <summary>
        /// This will remove the entity from the collection and optionally destroy the entity.
        /// It is worth noting if you try to remove an entity id that does not exist you will get an exception
        /// </summary>
        /// <param name="id">The Id of the entity you want to remove</param>
        /// <param name="disposeOnRemoval">If the entity should be disposed when removed (defaults to true)</param>
        void RemoveEntity(int id, bool disposeOnRemoval = true);
    }
}
