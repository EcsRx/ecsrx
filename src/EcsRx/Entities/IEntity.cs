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
        /// The Id of the entity
        /// </summary>
        /// <remarks>
        /// It is recommended you do not pass entities around and instead pass their ids around
        /// and then use the collection/observable group methods to get the entity from its id
        /// </remarks>
        Guid Id { get; }
        
        /// <summary>
        /// All the components which have been applied to this entity
        /// </summary>
        IEnumerable<IComponent> Components { get; }

        /// <summary>
        /// Adds a component instance to the entity
        /// </summary>
        /// <param name="component">The existing component</param>
        /// <returns>The component which has been added</returns>
        IComponent AddComponent(IComponent component);
        
        /// <summary>
        /// Adds a component to the entity based on its type with default setup
        /// </summary>
        /// <typeparam name="T">The type of component to apply</typeparam>
        /// <returns>The created component</returns>
        T AddComponent<T>() where T : class, IComponent, new(); 
        
        /// <summary>
        /// Removes a component instance from the entity
        /// </summary>
        /// <param name="component">The component instance to remove</param>
        /// <remarks>
        /// It is recommended if you can to batch component removals together
        /// </remarks>
        void RemoveComponent(IComponent component);
        
        /// <summary>
        /// Removes a component from the entity by its type
        /// </summary>
        /// <typeparam name="T">The type of entity to remove</typeparam>
        /// <remarks>
        /// It is recommended if you can to batch component removals together
        /// </remarks>
        void RemoveComponent<T>() where T : class, IComponent;
        
        /// <summary>
        /// Removes many component instances from the entity
        /// </summary>
        /// <param name="components">The components to remove</param>
        void RemoveComponents(params IComponent[] components);
        
        /// <summary>
        /// Removes all the components from the entity
        /// </summary>
        void RemoveAllComponents();
        
        /// <summary>
        /// Gets a component from the entity based upon its type or null if one cannot be found
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve</typeparam>
        /// <returns>The component instance if found, or null if not</returns>
        T GetComponent<T>() where T : class, IComponent;
        
        /// <summary>
        /// Gets a component from the entity based upon its type or null if one cannot be found
        /// </summary>
        /// <param name="componentType">The type of component to retrieve</param>
        /// <returns>The component instance if found, or null if not</returns>
        IComponent GetComponent(Type componentType);

        /// <summary>
        /// Checks to see if the entity contains a given component
        /// </summary>
        /// <typeparam name="T">Type of component to check for</typeparam>
        /// <returns>true if the component was found, false if it was not</returns>
        bool HasComponent<T>() where T : class, IComponent;
        
        /// <summary>
        /// Checks to see if the entity contains given components
        /// </summary>
        /// <param name="component">Types of component to check for</param>
        /// <returns>true if all the component was found, false if one or more is missing</returns>
        bool HasComponents(params Type[] component);
        
        /// <summary>
        /// Checks to see if the entity contains given components by their instances
        /// </summary>
        /// <param name="component">instances of component to check for</param>
        /// <returns>true if all the component was found, false if one or more is missing</returns>
        void AddComponents(params IComponent[] components);
    }
}
