using System.Collections.Generic;
using EcsRx.Groups.Accessors;

namespace EcsRx.Groups.Filtration
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