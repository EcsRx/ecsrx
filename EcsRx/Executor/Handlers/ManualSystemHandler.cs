using EcsRx.Pools;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    public class ManualSystemHandler : IConventionalSystemHandler<IManualSystem>
    {
        public IPoolManager PoolManager { get; }
        
        public ManualSystemHandler(IPoolManager poolManager)
        {
            PoolManager = poolManager;
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IManualSystem; }

        public void SetupSystem(IManualSystem system)
        {
            var groupAccessor = PoolManager.CreateObservableGroup(system.TargetGroup);
            system.StartSystem(groupAccessor);
        }

        public void DestroySystem(IManualSystem system)
        {
            var groupAccessor = PoolManager.CreateObservableGroup(system.TargetGroup);
            system.StopSystem(groupAccessor);
        }

        public void Dispose()
        {
        }
    }
}