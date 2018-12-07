using System.Collections.Generic;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    public interface IObservableGroupManager
    {
        IEnumerable<IObservableGroup> ObservableGroups { get; }
        IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds);
    }
}