using System.Linq;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Infrastructure;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class EntityDatabaseTransformer : IEntityDatabaseTransformer
    {
        public IEntityCollectionTransformer EntityCollectionTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public EntityDatabaseTransformer(IEntityCollectionTransformer entityCollectionTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionTransformer = entityCollectionTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }

        public object TransformTo(object original)
        {
            var entityDatabase = (IEntityDatabase)original;

            var entityCollections = entityDatabase.Collections
                .Select(EntityCollectionTransformer.TransformTo)
                .Cast<EntityCollectionData>()
                .ToList();

            return new EntityDatabaseData
            {
                EntityCollections = entityCollections
            };
        }

        public object TransformFrom(object converted)
        {
            var entityDatabaseData = (EntityDatabaseData) converted;
            var entityDatabase = new EntityDatabase(EntityCollectionFactory);
            entityDatabaseData.EntityCollections
                .Select(EntityCollectionTransformer.TransformFrom)
                .Cast<IEntityCollection>()
                .ForEachRun(entityDatabase.AddCollection);

            return entityDatabase;
        }
    }
}