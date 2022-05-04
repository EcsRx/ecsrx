using System.Linq;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class ToEntityDatabaseDataTransformer : IToEntityDatabaseDataTransformer
    {
        public IToEntityCollectionDataTransformer EntityCollectionDataTransformer { get; }

        public ToEntityDatabaseDataTransformer(IToEntityCollectionDataTransformer entityCollectionDataTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionDataTransformer = entityCollectionDataTransformer;
        }

        public object Transform(object original)
        {
            var entityDatabase = (IEntityDatabase)original;

            var entityCollections = entityDatabase.Collections
                .Select(EntityCollectionDataTransformer.Transform)
                .Cast<EntityCollectionData>()
                .ToList();

            return new EntityDatabaseData
            {
                EntityCollections = entityCollections
            };
        }
    }
}