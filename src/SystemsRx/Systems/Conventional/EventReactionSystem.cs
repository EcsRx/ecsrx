using System;
using SystemsRx.Events;
using EcsRx.MicroRx.Extensions;

namespace SystemsRx.Systems.Conventional
{
    public interface IReactToEventSystem<T> : ISystem
    {
        void Process(T eventData);
    }
    
    
    /// <summary>
    /// Event Reaction Systems are specifically made to act as event handlers,
    /// so when the given event comes in the system processes the event.
    /// </summary>
    /// <typeparam name="T">The type of event to handle</typeparam>
    /// <remarks>
    /// Given these systems have no dependency on a given group it is expected
    /// that the event payloads will contain all the data needed for the event
    /// to be handled, so if you need to access entities it is recommended
    /// that you pass their ids/references into the event data.
    /// </remarks>
    public abstract class EventReactionSystem<T> : IManualSystem
    {
        public IEventSystem EventSystem { get; }

        private IDisposable _subscription;

        protected EventReactionSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        public virtual void StartSystem()
        {
            _subscription = EventSystem.Receive<T>().Subscribe(EventTriggered);
        }

        public virtual void StopSystem()
        {
            _subscription.Dispose();
        }
        
        /// <summary>
        /// The logic to run when the event has been triggered
        /// </summary>
        /// <param name="eventData">The event data</param>
        public abstract void EventTriggered(T eventData);
    }
}