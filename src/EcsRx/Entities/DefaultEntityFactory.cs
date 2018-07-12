using System;
using EcsRx.Events;

namespace EcsRx.Entities
{
    public class DefaultEntityFactory : IEntityFactory
    {
        public IEntity Create(Guid? id = null)
        {
            if (!id.HasValue)
            { id = Guid.NewGuid(); }

            return new Entity(id.Value);
        }
    }
}