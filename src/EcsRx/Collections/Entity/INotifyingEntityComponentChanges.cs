using System;
using EcsRx.Collections.Events;
using R3;

namespace EcsRx.Collections.Entity
{
    public interface INotifyingEntityComponentChanges
    {
        Observable<ComponentsChangedEvent> EntityComponentsAdded { get; }
        Observable<ComponentsChangedEvent> EntityComponentsRemoving { get; }
        Observable<ComponentsChangedEvent> EntityComponentsRemoved { get; }
    }
}