using System;

namespace EcsRx.Groups
{
    public interface IGroup
    {
        Type[] RequiredComponents { get; }
        Type[] ExcludedComponents { get; }
    }
}