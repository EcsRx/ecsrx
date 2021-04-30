using System;
using System.Collections.Generic;
using SystemsRx.Attributes;
using SystemsRx.Extensions;
using SystemsRx.Scheduling;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.MicroRx.Extensions;

namespace SystemsRx.Executor.Handlers
{
    [Priority(6)]
    public class BasicSystemHandler : IConventionalSystemHandler
    {
        public readonly IUpdateScheduler UpdateScheduler;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public BasicSystemHandler(IUpdateScheduler updateScheduler)
        {
            UpdateScheduler = updateScheduler;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system is IBasicSystem; }

        public void SetupSystem(ISystem system)
        {
            var castSystem = (IBasicSystem)system;
            var subscription = UpdateScheduler.OnUpdate.Subscribe(castSystem.Execute);
            _systemSubscriptions.Add(system, subscription);
        }
        
        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}