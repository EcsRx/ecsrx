using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Groups.Observable;

namespace EcsRx.Extensions
{
    public static class IObservableGroupExtensions
    {
        public static IEnumerable<IEntity> Query(this IObservableGroup observableGroupAccesssor, IObservableGroupQuery query)
        { return query.Execute(observableGroupAccesssor); }
    }
}