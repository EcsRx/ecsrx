using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Groups.Accessors
{
    public interface IObservableGroup
    {
        ObservableGroupToken Token { get; }
        IEnumerable<IEntity> Entities { get; }
    }
}