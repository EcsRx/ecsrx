using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Accessors
{
    public interface IObservableGroupQuery
    {
        IEnumerable<IEntity> Execute(IObservableGroup observableGroup);
    }
}