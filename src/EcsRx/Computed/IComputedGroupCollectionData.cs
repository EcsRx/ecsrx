using System;
using System.Collections.Generic;

namespace EcsRx.Computed
{
    public interface IComputedGroupCollectionData<T> : IEnumerable<T>
    {
        IObservable<IEnumerable<T>> OnDataChanged { get; }
        IEnumerable<T> GetData();
    }
}