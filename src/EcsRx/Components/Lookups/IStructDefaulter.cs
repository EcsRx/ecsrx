using System;

namespace EcsRx.Components.Lookups
{
    public interface IStructDefaulter
    {
        ValueType GetDefault(int index);
        bool IsDefault<T>(T value, int index);
    }
}