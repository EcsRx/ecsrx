using EcsRx.Collections;

namespace EcsRx.Events
{
    public class CollectionRemovedEvent
    {
        public IEntityCollection EntityCollection { get; }

        public CollectionRemovedEvent(IEntityCollection entityCollection)
        {
            EntityCollection = entityCollection;
        }
    }
}