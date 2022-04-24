using System;
using System.Collections.Generic;

namespace EcsRx.Components.Lookups
{
    /// <summary>
    /// The Component Type Assigner interface is used to generate the internal lookup mappings
    /// for component types to ids.
    /// </summary>
    /// <remarks>
    /// The default implementation provided in this framework will use reflection to find all
    /// IComponent implementations within the AppDomain and then register them arbitrary values
    /// so this may not be consistent between runs.
    /// </remarks>
    public interface IComponentTypeAssigner
    {
        IReadOnlyDictionary<Type, int> GenerateComponentLookups();
    }
}