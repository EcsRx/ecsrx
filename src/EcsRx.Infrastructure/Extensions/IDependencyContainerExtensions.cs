using System;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Collections;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Infrastructure.Extensions
{
    public static class IDependencyContainerExtensions
    {
        /// <summary>
        /// Resolves an observable group
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="group">The group to observe</param>
        /// <returns>The observable group</returns>
        public static IObservableGroup ResolveObservableGroup(this IDependencyContainer container, IGroup group)
        {
            var collectionManager = container.Resolve<IObservableGroupManager>();
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
            var collectionManager = container.Resolve<IObservableGroupManager>();
            var group = new Group(componentTypes);
            return collectionManager.GetObservableGroup(group);
        }
    }
}