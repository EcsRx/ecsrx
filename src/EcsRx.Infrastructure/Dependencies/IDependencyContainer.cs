using System.Collections.Generic;

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
        /// <param name="configuration">Optional configuration</param>
        /// <typeparam name="TFrom">Type to bind from</typeparam>
        /// <typeparam name="TTo">Type to bind to</typeparam>
        void Bind<TFrom, TTo>(BindingConfiguration configuration = null) where TTo : TFrom;
        
        /// <summary>
        /// Bind to itself, useful for concrete classes to themselves
        /// </summary>
        /// <param name="configuration">Optional configuration</param>
        /// <typeparam name="T">Both source and destination binding, i.e concrete class</typeparam>
        void Bind<T>(BindingConfiguration configuration = null);

        /// <summary>
        /// Gets an instance of a given type from the underlying DI container
        /// </summary>
        /// <param name="name">Optional name of the binding</param>
        /// <typeparam name="T">Type of the object you want</typeparam>
        /// <returns>An instance of the given type</returns>
        T Resolve<T>(string name = null);
        
        /// <summary>
        /// Unbinds a type from the given context
        /// </summary>
        /// <typeparam name="T">The type to unbind</typeparam>
        void Unbind<T>();
        
        /// <summary>
        /// Gets an enumerable of a given type from the underlying DI container
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>All matching instances of that type within the underlying container</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Loads the given modules bindings into the underlying di container
        /// </summary>
        /// <typeparam name="T">Type of module to load</typeparam>
        void LoadModule<T>() where T : IDependencyModule, new();
        
        /// <summary>
        /// Loads the given modules bindings into the underlying di container
        /// </summary>
        /// <param name="module">Type of module to load</param>
        void LoadModule(IDependencyModule module);
    }
}