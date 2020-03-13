using System;
using System.Collections;

namespace EcsRx.Components
{
    /// <summary>
    /// Acts as a basic memory block for components of a specific type
    /// </summary>
    /// <remarks>This helps speed up access when you want components of the same type</remarks>
    /// <typeparam name="T">Type of component</typeparam>
    public interface IComponentPool<out T> : IComponentPool, IDisposable
        where T : IComponent
    {
        /// <summary>
        /// Contiguous block of memory for components 
        /// </summary>
        T[] Components { get; }
    }
    
    /// <summary>
    /// Acts as a basic memory block for components of a specific type
    /// </summary>
    /// <remarks>This helps speed up access for components of the same type</remarks>
    public interface IComponentPool : IEnumerable
    {
        /// <summary>
        /// Amount of components within the pool
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// The amount of indexes remaining in this pool before a resize is needed
        /// </summary>
        int IndexesRemaining { get; }
        
        /// <summary>
        /// To notify when a pool has extended
        /// </summary>
        IObservable<bool> OnPoolExtending { get; }
        
        /// <summary>
        /// Automatically expand the pool
        /// </summary>
        void Expand();
        
        /// <summary>
        /// Manually expand the pool
        /// </summary>
        /// <param name="amountToAdd"></param>
        void Expand(int amountToAdd);
        
        /// <summary>
        /// Set a component to a specific index
        /// </summary>
        /// <param name="index">the index to set</param>
        /// <param name="value">the component to use</param>
        void Set(int index, object value);
        
        /// <summary>
        /// Allocates space for the pools and indexes
        /// </summary>
        /// <returns>The id of the allocation</returns>
        int Allocate();
        
        /// <summary>
        /// Releases the component at the given index
        /// </summary>
        /// <param name="index"></param>
        void Release(int index);
    }
}