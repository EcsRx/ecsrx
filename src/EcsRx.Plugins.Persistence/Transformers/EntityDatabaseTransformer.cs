using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Infrastructure;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class EntityDatabaseTransformer : IEntityDatabaseTransformer
    {
        public IEntityCollectionDataTransformer EntityCollectionTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }
        public IEntityFactory EntityFactory { get; }


        public object TransformTo(object original)
        {
            var entityDatabase = (IEntityCollectionManager)original;

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
            /*
            var entityDatabaseData = (EntityDatabaseData) converted;
            var entityDatabase = new EntityCollectionManager();
            var collection = new EntityCollection(entityDatabaseData.CollectionId, EntityFactory);
            var entities = entityDatabaseData.Entities
                .Select(EntityTransformer.TransformFrom)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;*/
            return null;
        }
    }
}