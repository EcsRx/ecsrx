using System;

namespace EcsRx.Computed
{
    public interface IComputedGroupData<T>
    {
        IObservable<T> OnDataChanged { get; }
        T GetData();
    }
}