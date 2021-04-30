using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemsRx.Attributes;
using SystemsRx.Events;
using SystemsRx.Extensions;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.MicroRx.Extensions;

namespace SystemsRx.Executor.Handlers
{
    [Priority(6)]
    public class ReactToEventSystemHandler : IConventionalSystemHandler
    {
        public readonly IEventSystem EventSystem;
        public readonly IDictionary<ISystem, IDisposable> _systemSubscriptions;
        
        public ReactToEventSystemHandler(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
            _systemSubscriptions = new Dictionary<ISystem, IDisposable>();
        }

        public bool CanHandleSystem(ISystem system)
        { return system.IsReactToEventSystem(); }

        public MethodInfo MakeGenericMethod(Type classType, string methodName, Type dataType)
        {
            var receiveMethod = classType.GetMethods().SingleOrDefault(x => x.Name == methodName);
            return receiveMethod.MakeGenericMethod(dataType);
        }
        
        public MethodInfo GetGenericEventReceiveMethod(Type eventType)
        { return MakeGenericMethod(EventSystem.GetType(), "Receive", eventType); }

        public MethodInfo GetGenericSubscriptionMethod(Type eventType)
        { return MakeGenericMethod(typeof(IObservableExtensions), "Subscribe", eventType); }

        public MethodInfo GetGenericEventProcessMethod(ISystem system, Type eventType)
        { return system.GetType().GetMethods().SingleOrDefault(x => x.Name == "Process"); }
        
        public void SetupSystem(ISystem system)
        {
            var eventType = system.GetType().GetMatchingInterfaceGenericTypes(typeof(IReactToEventSystem<>));
            var receiveMethod = GetGenericEventReceiveMethod(eventType);
            var subscriptionMethod = GetGenericSubscriptionMethod(eventType);
            var systemProcessMethod = GetGenericEventProcessMethod(system, eventType);
            var systemProcessAction = Delegate.CreateDelegate(eventType, systemProcessMethod);
            
            var observable = receiveMethod.Invoke(EventSystem, new object[0]);
            var disposable = (IDisposable)subscriptionMethod.Invoke(null, new[] {observable, systemProcessAction});
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