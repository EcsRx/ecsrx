using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Groups.Accessors;

namespace EcsRx.Extensions
{
    public static class IGroupAccessorExtensions
    {
        public static IEnumerable<IEntity> Query(this IGroupAccessor groupAccesssor, IGroupAccessorQuery query)
        { return query.Execute(groupAccesssor); }
    }
}