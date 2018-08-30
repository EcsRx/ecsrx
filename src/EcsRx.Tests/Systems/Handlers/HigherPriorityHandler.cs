using EcsRx.Attributes;
using EcsRx.Executor.Handlers;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems.Handlers
{
    [Priority(-1)]
    public class HigherPriorityHandler : IConventionalSystemHandler
    {
        public void Dispose() {}
        public bool CanHandleSystem(ISystem system) => true;
        public void SetupSystem(ISystem system) {}
        public void DestroySystem(ISystem system){}
    }
}