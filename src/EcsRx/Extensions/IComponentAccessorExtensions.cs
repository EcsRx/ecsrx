using EcsRx.Components;
using EcsRx.Components.Accessor;
using EcsRx.Entities;

namespace EcsRx.Extensions
{
    public static class IComponentAccessorExtensions
    {
        public static ref T Add<T>(this IComponentAccessor<T> accessor, IEntity entity)
            where T : IComponent, new() => ref entity.AddComponent<T>(accessor.ComponentTypeId);
    }
}