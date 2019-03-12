using System;
using System.Collections;

namespace EcsRx.Infrastructure.Dependencies
{
    /// <summary>
    /// This represents a cross platform way of managing dependencies.
    /// 
    /// It is up to the consumer to implement the actual underlying handler
    /// for unity it will be done out of the box with Zenject, but in other
    /// platforms like Monogame, Godot etc it would be up to the consumer
    /// to define what DI system they want to use and create an implementation
    /// themselves.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// This exposes the underlying DI container, but any calls to this directly
        /// will not be cross platform, so be weary if you need it or not.
        /// </summary>
        object NativeContainer { get; }

        /// <summary>
        /// Binds from one type to another, generally from an interface to a concrete class
        /// </summary>
        /// <param name="fromType">Type to bind from</param>
        /// <param name="toType">Type to bind to</param>
        /// <param name="configuration">Optional configuration</param>
        void Bind(Type fromType, Type toType, BindingConfiguration configuration = null);
               
        /// <summary>
        /// Bind the type to itself/instance/method, useful for concrete class bindings 
        /// </summary>
        /// <param name="type">The type to bind</param>
        /// <param name="configuration">Optional configuration</param>
        /// <remarks>This is useful for self binding concrete classes</remarks>
        void Bind(Type type, BindingConfiguration configuration = null);

        /// <summary>
        /// Checks to see if a binding exists in the container
        /// </summary>
        /// <param name="type">Type to check against</param>
        /// <param name="name">Optional name of the binding</param>
        /// <returns></returns>
        bool HasBinding(Type type, string name = null);

        /// <summary>
        /// Gets an instance of a given type from the underlying DI container
        /// </summary>
        /// <param name="type">Type of the object you want</param>
        /// <param name="name">Optional name of the binding</param>
        /// <returns>An instance of the given type</returns>
        object Resolve(Type type, string name = null);

        /// <summary>
        /// Unbinds a type from the container
        /// </summary>
        /// <param name="type">The type to unbind</param>
        void Unbind(Type type);
        
        /// <summary>
        /// Gets an enumerable of a given type from the underlying DI container
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>All matching instances of that type within the underlying container</returns>
        IEnumerable ResolveAll(Type type);
        
        /// <summary>
        /// Loads the given modules bindings into the underlying di container
        /// </summary>
        /// <param name="module">Type of module to load</param>
        void LoadModule(IDependencyModule module);
    }
}