using System.Linq;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Extensions;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class FromEntityDatabaseDataTransformer : IFromEntityDatabaseDataTransformer
    {
        public IFromEntityCollectionDataTransformer EntityCollectionDataTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public FromEntityDatabaseDataTransformer(IFromEntityCollectionDataTransformer entityCollectionDataTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionDataTransformer = entityCollectionDataTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }

        public object Transform(object converted)
        {
            var entityDatabaseData = (EntityDatabaseData) converted;
            var entityDatabase = new EntityDatabase(EntityCollectionFactory);
            entityDatabaseData.EntityCollections
                .Select(EntityCollectionDataTransformer.Transform)
                .Cast<IEntityCollection>()
                .ForEachRun(entityDatabase.AddCollection);

            return entityDatabase;
        }
    }
}