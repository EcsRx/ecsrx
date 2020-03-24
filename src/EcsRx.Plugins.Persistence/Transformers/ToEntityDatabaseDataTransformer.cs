using System.Linq;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Infrastructure;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class ToEntityDatabaseDataTransformer : IToEntityDatabaseDataTransformer
    {
        public IToEntityCollectionTransformer EntityCollectionTransformer { get; }

        public ToEntityDatabaseDataTransformer(IToEntityCollectionTransformer entityCollectionTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionTransformer = entityCollectionTransformer;
        }

        public object Transform(object original)
        {
            var entityDatabase = (IEntityDatabase)original;

            var entityCollections = entityDatabase.Collections
                .Select(EntityCollectionTransformer.Transform)
                .Cast<EntityCollectionData>()
                .ToList();

            return new EntityDatabaseData
            {
                EntityCollections = entityCollections
            };
        }
    }
}