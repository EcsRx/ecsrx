using EcsRx.Entities;
using EcsRx.Systems;

namespace EcsRx.Plugins.ReactiveSystems.Systems
{
    /// <summary>
    /// Teardown systems are run ONCE when an entity is ABOUT TO LEAVE a given group,
    /// this means the entity will still contain all components etc as this system
    /// is triggered just before components/entities are removed.
    /// </summary>
    /// <remarks>
    /// If the entity joins the group and leaves again it will re-trigger the teardown
    /// method on there so keep this in mind. You can also combine ISetupSystem and
    /// ITeardownSystem on the same implementation which is often useful.
    /// </remarks>
    public interface ITeardownSystem : ISystem
    {
        /// <summary>
        /// The teardown method to be run when the entity is leaving the group
        /// </summary>
        /// <param name="entity">The entity to teardown</param>
        void Teardown(IEntity entity);
    }
}