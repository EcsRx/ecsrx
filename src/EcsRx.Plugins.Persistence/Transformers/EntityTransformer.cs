using System.Linq;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class EntityTransformer : IEntityTransformer
    {
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }

        public EntityTransformer(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
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
            var entity = new Entity(entityData.EntityId, ComponentDatabase, ComponentTypeLookup);
            entity.AddComponents(entityData.Components.ToArray());
            return entity;
        }
    }
}