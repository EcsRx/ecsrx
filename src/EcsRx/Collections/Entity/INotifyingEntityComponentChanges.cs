using System;
using EcsRx.Collections.Events;

namespace EcsRx.Collections.Entity
{
    public interface INotifyingEntityComponentChanges
    {
        IObservable<ComponentsChangedEvent> EntityComponentsAdded { get; }
        IObservable<ComponentsChangedEvent> EntityComponentsRemoving { get; }
        IObservable<ComponentsChangedEvent> EntityComponentsRemoved { get; }
    }
}