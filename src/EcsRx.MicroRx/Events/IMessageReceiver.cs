using System;

namespace EcsRx.MicroRx.Events
{
    public interface IMessageReceiver
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IObservable<T> Receive<T>();
    }
}