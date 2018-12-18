using System.Collections;
using System.Collections.Generic;

namespace EcsRx.Collections
{
    public interface IExpandingArrayPool : IEnumerable
    {
        int Count { get; }
        int IndexesRemaining { get; }
        
        IReadOnlyList<T> AsReadOnly<T>();
        T[] AsArray<T>();
        
        T Get<T>(int index);
        void Set<T>(int index, T value);
        void Expand(int amountToAdd);
        void Set(int index, object value);
        
        int Allocate();
        void Release(int index);
    }
}