using EcsRx.Entities;
using SystemsRx.Scheduling;

namespace EcsRx.Systems
{
    /// <summary>
    /// A system which processes every entity every update
    /// </summary>
    /// <remarks>
    /// This relies upon the underlying IObservableScheduler implementation and
    /// is by default aiming for 60 updates per second.
    /// </remarks>
    public interface IBasicEntitySystem : IGroupSystem
    {
        /// <summary>
        /// The processor to handle the entity
        /// </summary>
        /// <param name="entity">The entity to process</param>
        /// <param name="elapsedTime">The elapsedTime since last update</param>
        void Process(IEntity entity, ElapsedTime elapsedTime);
    }
}