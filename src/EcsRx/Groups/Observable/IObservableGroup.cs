using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    /// </summary>
    /// A maintained collection of entities which match a given group
    /// <summary>
    /// <remarks>
    /// Implements IEnumerable so you can just enumerate the entities within the group.
    /// 
    /// This is quite often going to be a cached list of entities which
    /// is kept up to date based off events being reported, so it is often
    /// more performant to use this rather than querying a collection directly.
    /// This can change based upon implementations though.
    /// </remarks>
    public interface IObservableGroup : IEnumerable<IEntity>

    {
        /// <summary>
        /// The underlying token that is used to describe the group
        /// </summary>
        /// <remarks>
        /// The token contains both components required and the specific collection
        /// it should be targetting if its been setup to only observe a given collection.
        /// </remarks>
        ObservableGroupToken Token { get; }
        
        /// <summary>
        /// Event stream for when an entity has been added to this group
        /// </summary>
        IObservable<IEntity> OnEntityAdded { get; }
        
        /// <summary>
        /// Event stream for when an entity has been removed from this group
        /// </summary>
        IObservable<IEntity> OnEntityRemoved { get; }
        
        /// <summary>
        /// Event stream for when an entity is about to be removed from this group
        /// </summary>
        IObservable<IEntity> OnEntityRemoving { get; }
    }
}