using EcsRx.Entities;
using EcsRx.Events;

namespace EcsRx.Collections
{
    public class DefaultEntityCollectionFactory : IEntityCollectionFactory
    {
        private readonly IEntityFactory _entityFactory;

        public DefaultEntityCollectionFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public IEntityCollection Create(string name)
        {
            return new EntityCollection(name, _entityFactory);
        }
    }
}