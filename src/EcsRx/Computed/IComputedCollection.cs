using System;
using System.Collections.Generic;
using EcsRx.Events;

namespace EcsRx.Computed
{
    public interface IComputedCollection<T> : IComputed<IEnumerable<T>>, IEnumerable<T>
    {
        T this[int index] {get;}
        int Count { get; }
        
        IObservable<CollectionElementChangedEvent<T>> OnAdded { get; }
        IObservable<CollectionElementChangedEvent<T>> OnRemoved { get; }
        IObservable<CollectionElementChangedEvent<T>> OnUpdated { get; }
    }
}