using System;
using System.Collections.Generic;

namespace EcsRx.Components.Lookups
{
    public interface IComponentTypeAssigner
    {
        IReadOnlyDictionary<Type, int> GenerateComponentLookups();
    }
}