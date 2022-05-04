using System;
using EcsRx.Collections.Events;

namespace EcsRx.Collections.Entity
{
    public interface INotifyingEntityCollection
    {
        IObservable<CollectionEntityEvent> EntityAdded { get; }
        IObservable<CollectionEntityEvent> EntityRemoved { get; }
    }
}