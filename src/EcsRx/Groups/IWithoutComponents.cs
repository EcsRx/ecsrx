using System;
using System.Collections.Generic;

namespace EcsRx.Groups
{
    public interface IWithoutComponents
    {
        IEnumerable<Type> WithoutComponents { get; }
    }
}