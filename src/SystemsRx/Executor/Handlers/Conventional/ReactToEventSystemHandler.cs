using System;
using System.Collections.Generic;
using System.Reflection;
using SystemsRx.Attributes;
using SystemsRx.Events;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.MicroRx.Extensions;

namespace SystemsRx.Executor.Handlers.Conventional
{
    [Priority(6)]
    public class ReactToEventSystemHandler : IConventionalSystemHandler
    {
        private readonly MethodInfo _setupSystemGenericMethodInfo;
        public readonly IEventSystem EventSystem;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;

        public ReactToEventSystemHandler(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
            _setupSystemGenericMethodInfo = typeof(ReactToEventSystemHandler).GetMethod(nameof(SetupSystemGeneric), BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public bool CanHandleSystem(ISystem system)
        { return system.IsReactToEventSystem(); }
        
        public Type GetEventTypeFromSystem(ISystem system)
        { return system.GetGenericDataType(typeof(IReactToEventSystem<>)); }

        public void SetupSystem(ISystem system)
        {
            var eventType = GetEventTypeFromSystem(system);
            _setupSystemGenericMethodInfo.MakeGenericMethod(eventType).Invoke(this, new object[] { system });
        }

        private void SetupSystemGeneric<T>(IReactToEventSystem<T> system)
        {
            var disposable = EventSystem.Receive<T>().Subscribe(system.Process);
            _systemSubscriptions.Add(system, disposable);
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