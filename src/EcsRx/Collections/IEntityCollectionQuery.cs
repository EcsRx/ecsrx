using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Collections
{
    public interface IEntityCollectionQuery
    {
        IEnumerable<IEntity> Execute(IEnumerable<IEntity> entityList);
    }
}