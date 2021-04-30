using SystemsRx.Infrastructure;
using EcsRx.Collections;
using EcsRx.Collections.Database;

namespace EcsRx.Infrastructure
{
    public interface IEcsRxApplication : ISystemsRxApplication
    {
        /// <summary>
        /// The entity database, allows you to create and manage entity collections
        /// </summary>
        IEntityDatabase EntityDatabase { get; }
        
        /// <summary>
        /// The observable group manager, allows you to get observable groups
        /// </summary>
        IObservableGroupManager ObservableGroupManager { get; }
    }
}