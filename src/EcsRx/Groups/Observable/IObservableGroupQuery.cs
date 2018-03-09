using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public interface IObservableGroupQuery
    {
        IEnumerable<IEntity> Execute(IObservableGroup observableGroup);
    }
}