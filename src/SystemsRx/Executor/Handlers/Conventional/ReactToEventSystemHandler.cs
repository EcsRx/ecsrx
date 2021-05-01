using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemsRx.Attributes;
using SystemsRx.Events;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.MicroRx.Disposables;
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

        public Type[] GetMatchingInterfaces(ISystem system)
        { return system.GetGenericInterfacesFor(typeof(IReactToEventSystem<>)).ToArray(); }
        
        public Type GetEventTypeFromSystem(ISystem system)
        { return system.GetGenericDataType(typeof(IReactToEventSystem<>)); }

        public void SetupSystem(ISystem system)
        {
            var matchingInterfaces = GetMatchingInterfaces(system);
            var disposables = new List<IDisposable>();
            foreach (var matchingInterface in matchingInterfaces)
            {
                var eventType = matchingInterface.GetGenericArguments()[0];
                var disposable = (IDisposable)_setupSystemGenericMethodInfo.MakeGenericMethod(eventType).Invoke(this, new object[] { system });
                disposables.Add(disposable);
            }
            _systemSubscriptions.Add(system, new CompositeDisposable(disposables));
        }

        private IDisposable SetupSystemGeneric<T>(IReactToEventSystem<T> system)
        { return EventSystem.Receive<T>().Subscribe(system.Process); }
        
        public void DestroySystem(ISystem system)
        { _systemSubscriptions.RemoveAndDispose(system); }

        public void Dispose()
        {
            _systemSubscriptions.Values.DisposeAll();
            _systemSubscriptions.Clear();
        }
    }
}