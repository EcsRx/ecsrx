using System.Collections.Generic;

namespace EcsRx.Reactive.Collections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IEnumerable<T> source)
        {
            return new ReactiveCollection<T>(source);
        }
    }
}