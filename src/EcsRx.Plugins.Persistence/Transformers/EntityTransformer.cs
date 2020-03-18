using System.Linq;
using EcsRx.Entities;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class EntityTransformer : IEntityTransformer
    {
        public IEntityFactory EntityFactory { get; }

        public EntityTransformer(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        public object TransformTo(object original)
        {
            var entity = (IEntity)original;
            return new EntityData
            {
                EntityId = entity.Id,
                Components = entity.Components.ToList()
            };
        }

        public object TransformFrom(object converted)
        {
            var entityData = (EntityData) converted;
            var entity = EntityFactory.Create(entityData.EntityId);
            entity.AddComponents(entityData.Components.ToArray());
            return entity;
        }
    }
}