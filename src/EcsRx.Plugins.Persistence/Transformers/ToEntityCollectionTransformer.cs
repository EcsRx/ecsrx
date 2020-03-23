using System.Linq;
using EcsRx.Collections;
using EcsRx.Collections.Entity;
using EcsRx.Events;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class ToEntityCollectionTransformer : IToEntityCollectionTransformer
    {
        public IToEntityTransformer EntityTransformer { get; }

        public ToEntityCollectionTransformer(IToEntityTransformer entityTransformer)
        {
            EntityTransformer = entityTransformer;
        }

        public object Transform(object original)
        {
            var collection = (IEntityCollection)original;

            var entityData = collection
                .Select(EntityTransformer.Transform)
                .Cast<EntityData>()
                .ToList();

            return new EntityCollectionData
            {
                CollectionId = collection.Id,
                Entities = entityData
            };
        }

    }
}