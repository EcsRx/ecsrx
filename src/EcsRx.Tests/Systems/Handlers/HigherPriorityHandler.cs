using System;
using SystemsRx.Attributes;
using SystemsRx.Executor.Handlers;
using SystemsRx.Systems;
using SystemsRx.Types;

namespace EcsRx.Tests.Systems.Handlers
{
    [Priority(PriorityTypes.Higher)]
    public class HigherPriorityHandler : IConventionalSystemHandler
    {
        private Action _doSomethingOnSetup;
        private Action _doSomethingOnDestroy;

        public HigherPriorityHandler(Action doSomethingOnSetup = null, Action doSomethingOnDestroy = null)
        {
            _doSomethingOnSetup = doSomethingOnSetup;
            _doSomethingOnDestroy = doSomethingOnDestroy;
        }
        
        public void Dispose() {}
        public bool CanHandleSystem(ISystem system) => true;
        
        public void SetupSystem(ISystem system)
        { _doSomethingOnSetup?.Invoke(); }

        public void DestroySystem(ISystem system)
        { _doSomethingOnDestroy?.Invoke(); }
    }
}