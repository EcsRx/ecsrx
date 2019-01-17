using System;
using System.Collections.Generic;
using EcsRx.Events.Collections;

namespace EcsRx.Plugins.Computeds.Collections
{
    /// <summary>
    /// Represents a computed collection of elements
    /// </summary>
    /// <typeparam name="T">The data to contain</typeparam>
    public interface IComputedCollection<T> : IComputed<IEnumerable<T>>, IEnumerable<T>
    {
        /// <summary>
        /// Get an element by its index
        /// </summary>
        /// <remarks>
        /// In some implementations this may be its id not a sequential index
        /// </remarks>
        /// <param name="index">index/id of the element</param>
        T this[int index] {get;}

        /// <summary>
        /// How many elements are within the collection
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Fired when an element is added
        /// </summary>
        IObservable<CollectionElementChangedEvent<T>> OnAdded { get; }
 
        /// <summary>
        /// Fired when an element is removed
        /// </summary>
        IObservable<CollectionElementChangedEvent<T>> OnRemoved { get; }

        /// <summary>
        /// Fired when an element is updated
        /// </summary>
        IObservable<CollectionElementChangedEvent<T>> OnUpdated { get; }
    }
}