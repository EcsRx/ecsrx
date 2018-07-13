using System;
using System.Collections.Generic;

namespace EcsRx.Components
{
    public interface IComponentTypeAssigner
    {
        IEnumerable<Type> GetAllComponentTypes();
        IDictionary<Type, int> GenerateComponentLookups();
    }
}