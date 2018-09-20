using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Executor;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Plugins;

namespace EcsRx.Infrastructure
{
    public interface IEcsRxApplication
    {
        IDependencyContainer Container { get; }
        
        ISystemExecutor SystemExecutor { get; }
        IEventSystem EventSystem { get; }
        IEntityCollectionManager EntityCollectionManager { get; }
        IEnumerable<IEcsRxPlugin> Plugins { get; }
        
        void StartApplication();
    }
}