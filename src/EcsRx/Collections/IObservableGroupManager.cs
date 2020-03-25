using System.Collections.Generic;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    public interface IObservableGroupManager
    {
        IReadOnlyList<IObservableGroup> ObservableGroups { get; }
        IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds);
        
        /// <summary>
        /// Gets an ObservableGroup which will observe the given group and maintain a collection of
        /// entities which are applicable. This is the preferred way to access entities inside collections.
        /// </summary>
        /// <remarks>
        /// It is worth noting that IObservableGroup instances are cached within the manager, so if there is
        /// a request for an observable group targetting the same underlying components (not the IGroup instance, but
        /// the actual components the group cares about) it will return the existing group, if one does not exist
        /// it is created.
        /// </remarks>
        /// <param name="group">The group to match entities on</param>
        /// <param name="collectionIds">The collection names to use (defaults to null)</param>
        /// <returns>An IObservableGroup monitoring the group passed in</returns>
        IObservableGroup GetObservableGroup(IGroup group, params int[] collectionIds);
    }
}