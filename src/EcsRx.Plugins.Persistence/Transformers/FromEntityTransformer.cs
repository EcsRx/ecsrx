using EcsRx.Entities;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class FromEntityTransformer : IFromEntityTransformer
    {
        public IEntityFactory EntityFactory { get; }

        public FromEntityTransformer(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        public object Transform(object converted)
        {
            var entityData = (EntityData) converted;
            var entity = EntityFactory.Create(entityData.EntityId);
            entity.AddComponents(entityData.Components.ToArray());
            return entity;
        }
    }
}