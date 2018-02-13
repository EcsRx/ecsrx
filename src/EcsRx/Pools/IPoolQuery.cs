using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Pools
{
    public interface IPoolQuery
    {
        IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList);
    }
}