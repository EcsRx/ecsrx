using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Groups.Accessors
{
    public interface IGroupAccessor
    {
        GroupAccessorToken AccessorToken { get; }
        IEnumerable<IEntity> Entities { get; }
    }
}