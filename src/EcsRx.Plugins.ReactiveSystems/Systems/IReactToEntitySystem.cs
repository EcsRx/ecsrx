using System;
using EcsRx.Entities;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Systems
{
    /// <summary>
    /// React To Entity systems react as and when the entity needs to be processed,
    /// this means rather than batching all updates at once, it instead allows you
    /// to react on each entity individually, i.e when the entity has less than 50%
    /// hp.
    /// </summary>
    /// <remarks>
    /// If you do not need to react to each entity individually it is recommended you
    /// use a React To LookupGroup system as they have less overhead as there is only one
    /// subscription required rather than 1 per entity.
    /// </remarks>
    public interface IReactToEntitySystem : IGroupSystem
    {
        /// <summary>
        /// Returns and observable indicating when the system should execute for a given entity
        /// </summary>
        /// <param name="entity">The entity to react to</param>
        /// <returns>Observable indicating when the system should execute</returns>
        IObservable<IEntity> ReactToEntity(IEntity entity);

        /// <summary>
        /// The processor to handle the entity reaction
        /// </summary>
        /// <param name="entity">The entity to use</param>
        void Process(IEntity entity);
    }
}