namespace EcsRx.Pools
{
    public interface IPool<T>
    {
        /// <summary>
        /// The size to expand the pool by if needed
        /// </summary>
        int IncrementSize { get; }
        
        /// <summary>
        /// Allocates an instance in the pool for use
        /// </summary>
        /// <returns>An instance to use</returns>
        T AllocateInstance();
        
        /// <summary>
        /// Frees up the pooled item for re-allocation
        /// </summary>
        /// <param name="obj">The instance to put back in the pool</param>
        void ReleaseInstance(T obj);
    }
}