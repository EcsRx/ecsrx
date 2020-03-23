using System.Linq;
using EcsRx.Entities;
using EcsRx.Plugins.Persistence.Data;

namespace EcsRx.Plugins.Persistence.Transformers
{
    public class ToEntityTransformer : IToEntityTransformer
    {
        public object Transform(object original)
        {
            var entity = (IEntity)original;
            return new EntityData
            {
                EntityId = entity.Id,
                Components = entity.Components.ToList()
            };
        }
    }
}