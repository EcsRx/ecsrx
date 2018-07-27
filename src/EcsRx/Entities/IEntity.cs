using System;
using System.Collections.Generic;
using EcsRx.Components;

namespace EcsRx.Entities
{
    /// <summary>
    /// A container for components, its only job is to let you compose components
    /// </summary>
    public interface IEntity : IDisposable
    {
        /// <summary>
        /// Triggered every time components are added to the entity
        /// </summary>
        /// <remarks>
        /// If you are adding components individually it will be fired once per interaction, its better to batch them
        /// </remarks>
        IObservable<Type[]> ComponentsAdded { get; }
        
        /// <summary>
        /// Triggered every time components are about to be removed from the entity
        /// </summary>
        /// <remarks>
        /// If you are removing components individually it will be fired once per interaction, its better to batch them
        /// </remarks>
        IObservable<Type[]> ComponentsRemoving { get; }
        
        /// <summary>
        /// Triggered every time components have been removed removed from the entity
        /// </summary>
        /// <remarks>
        /// If you are removing components individually it will be fired once per interaction, its better to batch them
        /// </remarks>
        IObservable<Type[]> ComponentsRemoved { get; }
        
        /// <summary>
        /// The Id of the entity
        /// </summary>
        /// <remarks>
        /// It is recommended you do not pass entities around and instead pass their ids around
        /// and then use the collection/observable group methods to get the entity from its id
        /// </remarks>
        int Id { get; }
        
        /// <summary>
        /// All the components which have been applied to this entity
        /// </summary>
        IEnumerable<IComponent> Components { get; }
       
        /// <summary>
        /// Removes component types from the entity
        /// </summary>
        /// <param name="componentsTypes">The component types to remove</param>
        void RemoveComponents(params Type[] componentsTypes);
        
        /// <summary>
        /// Removes all the components from the entity
        /// </summary>
        void RemoveAllComponents();
               
        /// <summary>
        /// Gets a component from the entity based upon its type or null if one cannot be found
        /// </summary>
        /// <param name="componentType">The type of component to retrieve</param>
        /// <returns>The component instance if found, or null if not</returns>
        IComponent GetComponent(Type componentType);
        
        /// <summary>
        /// Checks to see if the entity contains given components by their instances
        /// </summary>
        /// <param name="components">instances of component to check for</param>
        /// <returns>true if all the component was found, false if one or more is missing</returns>
        void AddComponents(params IComponent[] components);
        
        /// <summary>
        /// Checks to see if the entity contains the given component type
        /// </summary>
        /// <param name="componentType">Type of component to look for</param>
        /// <returns>true if the component can be found, false if it cant be</returns>
        bool HasComponent(Type componentType);
    }
}
