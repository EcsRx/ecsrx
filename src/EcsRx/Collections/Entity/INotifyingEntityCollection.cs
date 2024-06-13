using System;
using EcsRx.Collections.Events;
using R3;

namespace EcsRx.Collections.Entity
{
    public interface INotifyingEntityCollection
    {
        Observable<CollectionEntityEvent> EntityAdded { get; }
        Observable<CollectionEntityEvent> EntityRemoved { get; }
    }
}