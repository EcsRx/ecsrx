using System;
using EcsRx.Events;
using EcsRx.Groups.Accessors;
using EcsRx.Groups;

namespace EcsRx.Systems.Custom
{
    public abstract class EventReactionSystem<T> : IManualSystem
    {
        public virtual IGroup TargetGroup => new EmptyGroup();
        public IEventSystem EventSystem { get; }

        private IDisposable _subscription;

        protected EventReactionSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        public virtual void StartSystem(IObservableGroup observableGroup)
        {
            _subscription = EventSystem.Receive<T>().Subscribe(EventTriggered);
        }

        public virtual void StopSystem(IObservableGroup observableGroup)
        {
            _subscription.Dispose();
        }
        
        public abstract void EventTriggered(T eventData);
    }
}