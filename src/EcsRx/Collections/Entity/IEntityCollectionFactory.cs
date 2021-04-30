using SystemsRx.Factories;

namespace EcsRx.Collections.Entity
{
    /// <summary>
    /// Creates an entity collection for a given name
    /// </summary>
    /// <remarks>
    /// This is meant to be replaceable so you can create your own implementation and replace for using
    /// your own entity collection implementations
    /// </remarks>
    public interface IEntityCollectionFactory : IFactory<int, IEntityCollection>
    {
        
    }
}