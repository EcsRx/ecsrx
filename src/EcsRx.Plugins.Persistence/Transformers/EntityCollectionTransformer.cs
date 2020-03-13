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
        public IEventSystem EventSystem { get; }
        public IEntityFactory EntityFactory { get; }

        public EntityCollectionTransformer(IEntityTransformer entityTransformer, IEventSystem eventSystem, IEntityFactory entityFactory)
        {
            EntityTransformer = entityTransformer;
            EventSystem = eventSystem;
            EntityFactory = entityFactory;
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
            var collection = new EntityCollection(collectionData.CollectionId, EntityFactory);
            var entities = collectionData.Entities
                .Select(EntityTransformer.TransformFrom)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;
        }
    }
}