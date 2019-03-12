using System.Collections.Generic;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    public interface IObservableGroupManager
    {
        IReadOnlyList<IObservableGroup> ObservableGroups { get; }
        IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds);
    }
}