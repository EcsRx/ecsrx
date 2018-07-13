using System;
using System.Collections.Generic;

namespace EcsRx.Components
{
    public interface IComponentTypeAssigner
    {
        IReadOnlyDictionary<Type, int> GenerateComponentLookups();
    }
}