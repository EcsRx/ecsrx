using System;
using EcsRx.Events.Collections;

namespace EcsRx.Collections.Entity
{
    public interface INotifyingEntityCollection
    {
        IObservable<CollectionEntityEvent> EntityAdded { get; }
        IObservable<CollectionEntityEvent> EntityRemoved { get; }

        IObservable<ComponentsChangedEvent> EntityComponentsAdded { get; }
        IObservable<ComponentsChangedEvent> EntityComponentsRemoving { get; }
        IObservable<ComponentsChangedEvent> EntityComponentsRemoved { get; }
    }
}