using System;
using EcsRx.Collections;
using EcsRx.Computed;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Infrastructure.Dependencies;

namespace EcsRx.Infrastructure.Extensions
{
    public static class IDependencyContainerExtensions
    {
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