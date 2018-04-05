using EcsRx.Entities;
using EcsRx.Events;

namespace EcsRx.Collections
{
    public class DefaultEntityCollectionFactory : IEntityCollectionFactory
    {
        private readonly IEntityFactory _entityFactory;
        private readonly IEventSystem _eventSystem;

        public DefaultEntityCollectionFactory(IEntityFactory entityFactory, IEventSystem eventSystem)
        {
            _entityFactory = entityFactory;
            _eventSystem = eventSystem;
        }

        public IEntityCollection Create(string name)
        {
            return new EntityCollection(name, _entityFactory, _eventSystem);
        }
    }
}