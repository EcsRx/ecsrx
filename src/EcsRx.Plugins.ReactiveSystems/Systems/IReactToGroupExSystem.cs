namespace EcsRx.Plugins.ReactiveSystems.Systems
{
    public interface IReactToGroupExSystem : IReactToGroupSystem
    {
        /// <summary>
        /// Triggered before the group is processed
        /// </summary>
        void BeforeProcessing();
        
        /// <summary>
        /// Triggered after the group is processed
        /// </summary>
        void AfterProcessing();
    }
}