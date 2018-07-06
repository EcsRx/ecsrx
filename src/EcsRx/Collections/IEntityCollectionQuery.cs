using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Collections
{
    /// <summary>
    /// A pre made query which will extract relevant entities from the entity collection, this is useful
    /// for wrapping up complex logic. i.e Get all entities who have a component with Health > 25%
    /// </summary>
    /// <remarks>
    /// This will run on the live data so every query execution will cause an enumeration through
    /// the collection, this is often undesired for performance reasons but is useful for one off
    /// style queries.
    /// </remarks>
    public interface IEntityCollectionQuery
    {
        /// <summary>
        /// Acts as a way to filter the entity data.
        /// </summary>
        /// <remarks>
        /// This is often called automatically by other methods which act on the collection itself so its
        /// rare that you would ever need to Execute yourself manually.
        /// </remarks>
        /// <param name="entityList">The list of entities to enumerate</param>
        /// <returns>A filtered collection of entities to enumerate through</returns>
        IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList);
    }
}