using EcsRx.Attributes;
using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(5)]
    public class ManualSystemHandler : IConventionalSystemHandler
    {
        public IPoolManager PoolManager { get; }
        
        public ManualSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IManualSystem; }

        public void SetupSystem(ISystem system)
        {
            var groupAccessor = PoolManager.CreateObservableGroup(system.TargetGroup);
            var castSystem = (IManualSystem)system;
            castSystem.StartSystem(groupAccessor);
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            var groupAccessor = PoolManager.CreateObservableGroup(system.TargetGroup);
            castSystem.StopSystem(groupAccessor);
        }

        public void Dispose()
        {}
    }
}