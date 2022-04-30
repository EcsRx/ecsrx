using System;
using EcsRx.Events.Collections;

namespace EcsRx.Collections.Entity
{
    public interface INotifyingEntityComponentChanges
    {
        IObservable<ComponentsChangedEvent> EntityComponentsAdded { get; }
        IObservable<ComponentsChangedEvent> EntityComponentsRemoving { get; }
        IObservable<ComponentsChangedEvent> EntityComponentsRemoved { get; }
    }
}