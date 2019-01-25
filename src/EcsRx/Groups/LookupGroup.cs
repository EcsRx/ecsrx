using System;

namespace EcsRx.Groups
{
    public struct LookupGroup : IEquatable<LookupGroup>
    {
        public int[] RequiredComponents { get; }
        public int[] ExcludedComponents { get; }

        public LookupGroup(int[] requiredComponents, int[] excludedComponents)
        {
            RequiredComponents = requiredComponents;
            ExcludedComponents = excludedComponents;
        }

        public bool Equals(LookupGroup other)
        {
            return RequiredComponents == other.RequiredComponents
                   && ExcludedComponents == other.ExcludedComponents;
        }
        
        public override int GetHashCode()
        {
            var requiredHash = RequiredComponents?.GetHashCode() ?? 0;
            var excludedHash = ExcludedComponents?.GetHashCode() ?? 0;
            return requiredHash ^ excludedHash;
        }

    }
}