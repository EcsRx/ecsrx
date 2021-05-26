using System;
using System.Collections.Generic;
using EcsRx.MicroRx;

namespace EcsRx.ReactiveData.Collections
{
    public interface IReadOnlyReactiveCollection<T> : IReadOnlyList<T>, IDisposable
    {
        IObservable<CollectionAddEvent<T>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<CollectionMoveEvent<T>> ObserveMove();
        IObservable<CollectionRemoveEvent<T>> ObserveRemove();
        IObservable<CollectionReplaceEvent<T>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }
}