using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(5)]
    public class ManualSystemHandler : IConventionalSystemHandler
    {
        public IEntityCollectionManager EntityCollectionManager { get; }
        
        public ManualSystemHandler(IEntityCollectionManager entityCollectionManager)
        {
            EntityCollectionManager = entityCollectionManager;
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IManualSystem; }

        public void SetupSystem(ISystem system)
        {
            var groupAccessor = EntityCollectionManager.GetObservableGroup(system.TargetGroup);
            var castSystem = (IManualSystem)system;
            castSystem.StartSystem(groupAccessor);
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            var groupAccessor = EntityCollectionManager.GetObservableGroup(system.TargetGroup);
            castSystem.StopSystem(groupAccessor);
        }

        public void Dispose()
        {}
    }
}