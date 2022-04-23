using EcsRx.Entities;

namespace EcsRx.Components.Accessor
{
    /// <summary>
    /// Represents an optimised way to interact with a component type on an entity
    /// </summary>
    /// <typeparam name="T">The type of the component</typeparam>
    /// <remarks>
    /// In most cases you can just use the provided extension methods on IEntity, however for
    /// optimisations purposes this provides you a streamlined way to caching the component
    /// type id and relaying calls through to the underlying entity.
    /// </remarks>
    public interface IComponentAccessor<T>
    {
        int ComponentTypeId { get; }
        bool Has(IEntity entity);
        T Get(IEntity entity);
        bool TryGet(IEntity entity, out T component);
        void Remove(IEntity entity);
    }
}