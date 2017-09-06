using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Accessors
{
    public interface IGroupAccessorQuery
    {
        IEnumerable<IEntity> Execute(IGroupAccessor groupAccessor);
    }
}