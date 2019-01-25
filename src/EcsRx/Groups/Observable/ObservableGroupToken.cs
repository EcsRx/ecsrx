using System;

namespace EcsRx.Groups.Observable
{
    public struct ObservableGroupToken : IEquatable<ObservableGroupToken>
    {
        public LookupGroup LookupGroup { get; }
        public int[] CollectionIds { get; }

        public ObservableGroupToken(int[] withComponents, int[] withoutComponents, params int[] collectionIds)
        {
            LookupGroup = new LookupGroup(withComponents, withoutComponents);
            CollectionIds = collectionIds;
        }
        
        public ObservableGroupToken(LookupGroup lookupGroup, params int[] collectionIds)
        {
            LookupGroup = lookupGroup;
            CollectionIds = collectionIds;
        }

        public bool Equals(ObservableGroupToken other)
        {
            return LookupGroup.RequiredComponents == other.LookupGroup.RequiredComponents
                   && LookupGroup.ExcludedComponents == other.LookupGroup.ExcludedComponents
                   && CollectionIds == other.CollectionIds;
        }
        
        public override int GetHashCode()
        {
            var requiredHash = LookupGroup.RequiredComponents?.GetHashCode() ?? 0;
            var excludedHash = LookupGroup.ExcludedComponents?.GetHashCode() ?? 0;
            var poolHash = CollectionIds?.GetHashCode() ?? 0;
            return requiredHash ^ excludedHash ^ poolHash;
        }
        
    }
}