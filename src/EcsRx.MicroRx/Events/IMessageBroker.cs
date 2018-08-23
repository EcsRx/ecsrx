using System;

/*
 *    This code was taken from UniRx project by neuecc
 *    https://github.com/neuecc/UniRx
 */
namespace EcsRx.MicroRx.Events
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        void Publish<T>(T message);
    }

    public interface IMessageReceiver
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IObservable<T> Receive<T>();
    }

    public interface IMessageBroker : IMessagePublisher, IMessageReceiver
    {
    }
}