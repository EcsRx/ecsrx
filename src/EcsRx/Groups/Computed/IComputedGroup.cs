using System.Collections.Generic;
using EcsRx.Groups.Observable;

namespace EcsRx.Groups.Computed
{
    public interface IComputedGroup
    {
        IObservableGroup ObservableGroup { get; }
    }

    public interface IComputedGroup<T> : IComputedGroup
    {
        IEnumerable<T> Filter();
    }

    public interface IComputedGroup<TOutput, TInput> : IComputedGroup
    {
        IEnumerable<TOutput> Filter(TInput input);
    }
}