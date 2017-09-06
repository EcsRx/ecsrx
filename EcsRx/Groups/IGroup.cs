using System;
using System.Collections.Generic;

namespace EcsRx.Groups
{
    public interface IGroup
    {
        IEnumerable<Type> TargettedComponents { get; }
    }
}