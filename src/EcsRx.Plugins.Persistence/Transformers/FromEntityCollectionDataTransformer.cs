using System.Linq;
using SystemsRx.Extensions;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class FromEntityCollectionDataTransformer : IFromEntityCollectionDataTransformer
    {
        public IFromEntityDataTransformer EntityDataTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public FromEntityCollectionDataTransformer(IFromEntityDataTransformer entityDataTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityDataTransformer = entityDataTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }
        
        public object Transform(object converted)
        {
            var collectionData = (EntityCollectionData) converted;
            var collection = EntityCollectionFactory.Create(collectionData.CollectionId);
            var entities = collectionData.Entities
                .Select(EntityDataTransformer.Transform)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;
        }
    }
}