using EcsRx.Entities;
using EcsRx.Extensions;

namespace EcsRx.Components.Accessor
{
    public class ComponentAccessor<T> : IComponentAccessor<T>
    {
        public int ComponentTypeId { get; }

        public ComponentAccessor(int componentTypeTypeId)
        { ComponentTypeId = componentTypeTypeId; }

        public bool Has(IEntity entity) => entity.HasComponent(ComponentTypeId);
        public T Get(IEntity entity) => (T)entity.GetComponent(ComponentTypeId);
        public void Remove(IEntity entity) => entity.RemoveComponents(ComponentTypeId);
        
        public bool TryGet(IEntity entity, out T component)
        {
            if (Has(entity))
            {
                component = Get(entity);
                return true;
            }

            component = default;
            return false;
        }
    }
}