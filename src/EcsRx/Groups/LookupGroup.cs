using System;
using System.Collections;
using System.Linq;

namespace EcsRx.Groups
{
    public readonly struct LookupGroup
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
            return StructuralComparisons.StructuralEqualityComparer.Equals(RequiredComponents, other.RequiredComponents) && 
                   StructuralComparisons.StructuralEqualityComparer.Equals(ExcludedComponents, other.ExcludedComponents);
        }

        public override bool Equals(object obj)
        {
            return obj is LookupGroup other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((RequiredComponents != null ? StructuralComparisons.StructuralEqualityComparer.GetHashCode(RequiredComponents) : 0) * 397) ^ (ExcludedComponents != null ? StructuralComparisons.StructuralEqualityComparer.GetHashCode(ExcludedComponents) : 0);
            }
        }

        public static bool operator ==(LookupGroup left, LookupGroup right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LookupGroup left, LookupGroup right)
        {
            return !left.Equals(right);
        }
    }
}