using System.Collections.Generic;

namespace EcsRx.Collections
{
    public interface IExpandingArray
    {
        int Count { get; }
        
        IReadOnlyList<T> AsReadOnly<T>();
        T[] GetArray<T>();
        T GetItem<T>(int index);
        void SetItem<T>(int index, T value);
        void Expand(int amountToAdd);
    }
}