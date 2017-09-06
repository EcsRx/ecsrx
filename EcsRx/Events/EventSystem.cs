using System;
using EcsRx.Reactive;

namespace EcsRx.Events
{
    public class EventSystem : IEventSystem
    {
        public IMessageBroker MessageBroker { get; }

        public EventSystem(IMessageBroker messageBroker)
        { MessageBroker = messageBroker; }

        public void Publish<T>(T message)
        { MessageBroker.Publish(message); }

        public IObservable<T> Receive<T>()
        { return MessageBroker.Receive<T>(); }

    }
}