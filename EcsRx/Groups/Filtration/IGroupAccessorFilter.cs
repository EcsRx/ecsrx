using System.Collections.Generic;
using EcsRx.Groups.Accessors;

namespace EcsRx.Groups.Filtration
{
    public interface IGroupAccessorFilter
    {
        IGroupAccessor GroupAccessor { get; }
    }

    public interface IGroupAccessorFilter<T> : IGroupAccessorFilter
    {
        IEnumerable<T> Filter();
    }

    public interface IGroupAccessorFilter<TOutput, TInput> : IGroupAccessorFilter
    {
        IEnumerable<TOutput> Filter(TInput input);
    }
}