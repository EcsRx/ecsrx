using EcsRx.Attributes;
using EcsRx.Collections;
using EcsRx.Systems;

namespace EcsRx.Executor.Handlers
{
    [Priority(5)]
    public class ManualSystemHandler : IConventionalSystemHandler
    {
        public IObservableGroupManager ObservableGroupManager { get; }
        
        public ManualSystemHandler(IObservableGroupManager observableGroupManager)
        {
            ObservableGroupManager = observableGroupManager;
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IManualSystem; }

        public void SetupSystem(ISystem system)
        {
            var observableGroup = ObservableGroupManager.GetObservableGroup(system.Group);
            var castSystem = (IManualSystem)system;
            castSystem.StartSystem(observableGroup);
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            var observableGroup = ObservableGroupManager.GetObservableGroup(system.Group);
            castSystem.StopSystem(observableGroup);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}