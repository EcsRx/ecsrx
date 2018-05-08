using EcsRx.Collections;

namespace EcsRx.Events
{
    public class PoolRemovedEvent
    {
        public IEntityCollection EntityCollection { get; }

        public PoolRemovedEvent(IEntityCollection entityCollection)
        {
            EntityCollection = entityCollection;
        }
    }
}