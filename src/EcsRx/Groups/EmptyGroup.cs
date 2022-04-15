using System;

namespace EcsRx.Groups
{
    public class EmptyGroup : IGroup
    {
        public Type[] RequiredComponents { get; } = Array.Empty<Type>();
        public Type[] ExcludedComponents { get; } = Array.Empty<Type>();
    }
}