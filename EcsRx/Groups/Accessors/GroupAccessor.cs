using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Groups.Accessors
{
    public class GroupAccessor : IGroupAccessor
    {
        public GroupAccessorToken AccessorToken { get; private set; }
        public IEnumerable<IEntity> Entities { get; private set; }

        public GroupAccessor(GroupAccessorToken accessorToken, IEnumerable<IEntity> entities)
        {
            AccessorToken = accessorToken;
            Entities = entities;
        }
    }
}