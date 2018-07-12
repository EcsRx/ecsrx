using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Events
{
    public class CollectionAddedEvent
    {
        public IEntityCollection EntityCollection { get; }

        public CollectionAddedEvent(IEntityCollection entityCollection)
        {
            EntityCollection = entityCollection;
        }
    }
}