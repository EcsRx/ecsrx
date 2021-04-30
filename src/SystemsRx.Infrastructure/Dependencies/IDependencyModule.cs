namespace SystemsRx.Infrastructure.Dependencies
{
    /// <summary>
    /// This represents a cross platform DI module which contains
    /// contextual bindings
    /// </summary>
    /// <remarks>
    /// Although you can access the underlying DI container, you should only use
    /// the interfaces exposed methods, if you were to do anything on the native
    /// object then it will not be cross platform, which may be fine for your
    /// use cases, but it is something to keep in mind.
    /// </remarks>
    public interface IDependencyModule
    {
        /// <summary>
        /// The entry point where you can setup all your binding config
        /// </summary>
        /// <param name="container">The container to bind with</param>
        void Setup(IDependencyContainer container);
    }
}