using EcsRx.Factories;

namespace EcsRx.Entities
{
    /// <summary>
    /// Creates entities with given Ids
    /// </summary>
    /// <remarks>
    /// This is meant to be implemented if you want to create your own IEntity
    /// implementations as this acts as an abstraction layer over how the entities are created
    /// </remarks>
    public interface IEntityFactory : IFactory<int?, IEntity>
    {
        void Destroy(int entityId);
    }
}