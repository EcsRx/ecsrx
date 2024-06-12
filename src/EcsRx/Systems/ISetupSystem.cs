﻿using EcsRx.Entities;

namespace EcsRx.Systems
{
    /// <summary>
    /// Setup systems are run ONCE when an entity joins a given group,
    /// there is also some additional logic around handling predicates so if
    /// an entity matches a group BUT it has a predicate that is not met, the entity
    /// will be monitored and when the entity matches the group and the predicate
    /// passes it will then run the setup.
    /// </summary>
    /// <remarks>
    /// If the entity leaves the group and re-joins it will re-trigger the setup
    /// method on there so keep this in mind, and just because components match
    /// if there is a predicate that doesn't match then it wont be run until it
    /// does match.
    /// </remarks>
    public interface ISetupSystem : IGroupSystem
    {
        /// <summary>
        /// The logic run when the entity needs setting up
        /// </summary>
        /// <param name="entity">The entity to setup</param>
        void Setup(IEntity entity);
    }
}