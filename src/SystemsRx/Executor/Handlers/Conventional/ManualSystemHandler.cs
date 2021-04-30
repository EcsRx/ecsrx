using SystemsRx.Attributes;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;

namespace SystemsRx.Executor.Handlers.Conventional
{
    [Priority(5)]
    public class ManualSystemHandler : IConventionalSystemHandler
    {
        public bool CanHandleSystem(ISystem system)
        { return system is IManualSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            castSystem.StartSystem();
        }

        public void DestroySystem(ISystem system)
        {
            var castSystem = (IManualSystem)system;
            castSystem.StopSystem();
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}