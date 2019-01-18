using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Dependencies;

namespace EcsRx.Infrastructure.Extensions
{
    public static class IDependencyContainerExtensions
    {
                
        /// <summary>
        /// Binds from one type to another, generally from an interface to a concrete class
        /// </summary>
        /// <param name="container">The container to bind on</param>
        /// <param name="configuration">Optional configuration</param>
        /// <typeparam name="TFrom">Type to bind from</typeparam>
        /// <typeparam name="TTo">Type to bind to</typeparam>
        public static void Bind<TFrom, TTo>(this IDependencyContainer container, BindingConfiguration configuration = null) where TTo : TFrom
        { container.Bind(typeof(TFrom), typeof(TTo), configuration); }
        
        
        /// <summary>
        /// Binds from one type to another, generally from an interface to a concrete class
        /// </summary>
        /// <param name="container">The container to bind on</param>
        /// <param name="configuration">Optional configuration</param>
        /// <typeparam name="T">Source and possible destination binding, i.e concrete class</typeparam>
        /// <remarks>This is useful for self binding concrete classes</remarks>
        public static void Bind<T>(this IDependencyContainer container, BindingConfiguration configuration = null)
        { container.Bind(typeof(T), configuration); }
        
        /// <summary>
        /// Bind using a builder for fluent configuration
        /// </summary>
        /// <param name="container">container to act on</param>
        /// <param name="builderAction">configuration handler</param>
        /// <typeparam name="T">Type to bind to</typeparam>
        public static void Bind<T>(this IDependencyContainer container, Action<BindingBuilder<T>> builderAction)
        {
            var builder = new BindingBuilder<T>();
            builderAction(builder);
            var config = builder.Build();
            container.Bind<T>(config);
        }
        
        /// <summary>
        /// Bind using a builder for fluent configuration
        /// </summary>
        /// <param name="container">container to act on</param>
        /// <param name="builderAction">configuration handler</param>
        /// <typeparam name="TFrom">Type to bind from</typeparam>
        /// <typeparam name="TTo">Type to bind to</typeparam>
        public static void Bind<TFrom, TTo>(this IDependencyContainer container, Action<BindingBuilder> builderAction) where TTo : TFrom
        {
            var builder = new BindingBuilder();
            builderAction(builder);
            var config = builder.Build();
            container.Bind<TFrom, TTo>(config);
        }

        /// <summary>
        /// Checks to see if a binding exists in the container
        /// </summary>
        /// <param name="container">The container to check against</param>
        /// <param name="name">Optional name of the binding</param>
        /// <typeparam name="T">Type to check against</typeparam>
        /// <returns>True if the type has been bound, false if not</returns>
        public static bool HasBinding<T>(this IDependencyContainer container, string name = null)
        { return container.HasBinding(typeof(T), name); }

        /// <summary>
        /// Gets an instance of a given type from the underlying DI container
        /// </summary>
        /// <param name="container">The container to resolve from</param>
        /// <param name="name">Optional name of the binding</param>
        /// <typeparam name="T">Type of the object you want</typeparam>
        /// <returns>An instance of the given type</returns>
        public static T Resolve<T>(this IDependencyContainer container, string name = null)
        { return (T)container.Resolve(typeof(T), name); }
        
        /// <summary>
        /// Unbinds a type from the container
        /// </summary>
        /// <typeparam name="T">The type to unbind</typeparam>
        public static void Unbind<T>(this IDependencyContainer container)
        { container.Unbind(typeof(T)); }

        /// <summary>
        /// Gets an enumerable of a given type from the underlying DI container
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>All matching instances of that type within the underlying container</returns>
        public static IEnumerable<T> ResolveAll<T>(this IDependencyContainer container)
        { return container.ResolveAll(typeof(T)).Cast<T>(); }
        
        /// <summary>
        /// Loads the given modules bindings into the underlying di container
        /// </summary>
        /// <typeparam name="T">Type of module to load</typeparam>
        public static void LoadModule<T>(this IDependencyContainer container) where T : IDependencyModule, new()
        { container.LoadModule(new T()); }
        
        /// <summary>
        /// Resolves an observable group
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="group">The group to observe</param>
        /// <returns>The observable group</returns>
        public static IObservableGroup ResolveObservableGroup(this IDependencyContainer container, IGroup group)
        {
            var collectionManager = container.Resolve<IEntityCollectionManager>();
            return collectionManager.GetObservableGroup(group);
        }
        
        /// <summary>
        /// Resolves an observable group
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="componentTypes">The required components for the group to observe</param>
        /// <returns></returns>
        public static IObservableGroup ResolveObservableGroup(this IDependencyContainer container, params Type[] componentTypes)
        {
            var collectionManager = container.Resolve<IEntityCollectionManager>();
            var group = new Group(componentTypes);
            return collectionManager.GetObservableGroup(group);
        }
    }
}