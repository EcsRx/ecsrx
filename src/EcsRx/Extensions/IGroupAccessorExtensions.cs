using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Groups.Accessors;

namespace EcsRx.Extensions
{
    public static class IGroupAccessorExtensions
    {
        public static IEnumerable<IEntity> Query(this IObservableGroup observableGroupAccesssor, IObservableGroupQuery query)
        { return query.Execute(observableGroupAccesssor); }
    }
}