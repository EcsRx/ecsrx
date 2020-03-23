using System.Linq;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class FromEntityCollectionTransformer : IFromEntityCollectionTransformer
    {
        public IFromEntityTransformer EntityTransformer { get; }
        public IEntityCollectionFactory EntityCollectionFactory { get; }

        public FromEntityCollectionTransformer(IFromEntityTransformer entityTransformer, IEntityCollectionFactory entityCollectionFactory)
        {
            EntityTransformer = entityTransformer;
            EntityCollectionFactory = entityCollectionFactory;
        }
        
        public object Transform(object converted)
        {
            var collectionData = (EntityCollectionData) converted;
            var collection = EntityCollectionFactory.Create(collectionData.CollectionId);
            var entities = collectionData.Entities
                .Select(EntityTransformer.Transform)
                .Cast<IEntity>();
            
            entities.ForEachRun(collection.AddEntity);
            return collection;
        }
    }
}