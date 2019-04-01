using System;
using EcsRx.Entities;
using EcsRx.Groups.Observable;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Systems
{
    /// <summary>
    /// React To LookupGroup systems are the more common ECS style system,
    /// as they batch handle all applicable entities at once. This means
    /// you do not react to individual entities and instead react at the
    /// group level, be it every frame or time period.
    /// </summary>
    /// <remarks>
    /// This is the most common use case system, as it aligns with common
    /// ECS paradigms, i.e a system which triggers every update and runs
    /// on all applicable entities. If you need more control over individual
    /// entity reactions etc then look at ReactToEntity/Data systems.
    /// </remarks>
    public interface IReactToGroupSystem : ISystem
    {
        /// <summary>
        /// Dictates when the group should be processed
        /// </summary>
        /// <param name="observableGroup">The observable group to process</param>
        /// <returns>The observable chain containing the group</returns>
        /// <remarks>
        /// In most use cases you probably want to run this every update/interval
        /// </remarks>
        IObservable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup);
        
        /// <summary>
        /// The processor for the entity
        /// </summary>
        /// <param name="entity">The entity to process</param>
        void Process(IEntity entity);
    }
}