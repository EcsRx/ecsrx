using System;
using System.Collections.Generic;
using System.Reactive;

namespace EcsRx.ReactiveData.Collections
{
    public interface IReadOnlyReactiveCollection<T> : IReadOnlyList<T>
    {
        IObservable<CollectionAddEvent<T>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<CollectionMoveEvent<T>> ObserveMove();
        IObservable<CollectionRemoveEvent<T>> ObserveRemove();
        IObservable<CollectionReplaceEvent<T>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }
}