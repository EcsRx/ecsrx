using EcsRx.Entities;

namespace EcsRx.Blueprints
{
    /// <summary>
    /// Blueprints are a pre-defined setup routine for an entity, this is meant to setup required
    /// components and default values. Such as if you wanted to setup an NPC who needed both Moveable, Talkable
    /// components and default values around movement speed etc.
    /// </summary>
    /// <remarks>
    /// You *can* apply multiple blueprints after an entity has been created, but this is meant for you to be able to
    /// have a mixin style approach to setting up entity config data, it is not meant to replace an
    /// ISetupSystem which will do more than just setup entity config but also run logic on the component
    /// </remarks>
    public interface IBlueprint
    {
        /// <summary>
        /// Applies the given blueprint to the entity
        /// </summary>
        /// <param name="entity">The entity to be configured</param>
        void Apply(IEntity entity);
    }
}