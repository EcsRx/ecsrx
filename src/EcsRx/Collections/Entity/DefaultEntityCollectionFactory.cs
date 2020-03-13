using EcsRx.Entities;

namespace EcsRx.Collections.Entity
{
    public class DefaultEntityCollectionFactory : IEntityCollectionFactory
    {
        private readonly IEntityFactory _entityFactory;

        public DefaultEntityCollectionFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public IEntityCollection Create(int id)
        {
            return new EntityCollection(id, _entityFactory);
        }
    }
}