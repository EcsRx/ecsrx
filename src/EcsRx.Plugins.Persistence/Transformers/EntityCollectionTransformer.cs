using System.Linq;
using EcsRx.Collections;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class EntityCollectionTransformer : IEntityCollectionTransformer
    {
        public IEntityTransformer EntityTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public EntityCollectionTransformer(IEntityTransformer entityTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityTransformer = entityTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }

        public object TransformTo(object original)
        {
            var collection = (IEntityCollection)original;

            var entityData = collection
                .Select(EntityTransformer.TransformTo)
                .Cast<EntityData>()
                .ToList();

            return new EntityCollectionData
            {
                CollectionId = collection.Id,
                Entities = entityData
            };
        }

        public object TransformFrom(object converted)
        {
            var collectionData = (EntityCollectionData) converted;
            var collection = EntityCollectionFactory.Create(collectionData.CollectionId);
            var entities = collectionData.Entities
                .Select(EntityTransformer.TransformFrom)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;
        }
    }
}