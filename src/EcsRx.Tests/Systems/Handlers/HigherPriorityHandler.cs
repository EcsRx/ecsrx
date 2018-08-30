using System;
using EcsRx.Attributes;
using EcsRx.Executor.Handlers;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems.Handlers
{
    [Priority(100)]
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