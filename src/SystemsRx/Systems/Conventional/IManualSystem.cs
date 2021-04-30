namespace SystemsRx.Systems.Conventional
{
    /// <summary>
    /// Manual systems are the most basic system where you are provided
    /// a start and stop method which are run when the system is first
    /// registered, and the stop for when the system is removed.
    /// </summary>
    /// <remarks>
    /// There is an entire package (EcsRx.Systems) which contains the
    /// most common conventions for systems, so there is likely a more
    /// applicable interface in there.
    /// </remarks>
    public interface IManualSystem : ISystem
    {
        /// <summary>
        /// Run when the system has been registered
        /// </summary>
        void StartSystem();
        
        /// <summary>
        /// Run when the system has been removed
        /// </summary>
        void StopSystem();
    }
}