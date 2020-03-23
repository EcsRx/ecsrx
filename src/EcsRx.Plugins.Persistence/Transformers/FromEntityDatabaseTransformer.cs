using System.Linq;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Extensions;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class FromEntityDatabaseTransformer : IFromEntityDatabaseTransformer
    {
        public IFromEntityCollectionTransformer EntityCollectionTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public FromEntityDatabaseTransformer(IFromEntityCollectionTransformer entityCollectionTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionTransformer = entityCollectionTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }

        public object Transform(object converted)
        {
            var entityDatabaseData = (EntityDatabaseData) converted;
            var entityDatabase = new EntityDatabase(EntityCollectionFactory);
            entityDatabaseData.EntityCollections
                .Select(EntityCollectionTransformer.Transform)
                .Cast<IEntityCollection>()
                .ForEachRun(entityDatabase.AddCollection);

            return entityDatabase;
        }
    }
}