using System;
using System.Collections.Generic;

namespace EcsRx.Groups
{
    public interface IGroup
    {
        Type[] RequiredComponents { get; }
        Type[] ExcludedComponents { get; }
    }
}