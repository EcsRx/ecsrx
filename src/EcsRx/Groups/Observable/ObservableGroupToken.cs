using System;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupToken
    {
        public ILookupGroup LookupGroup { get; }
        public string CollectionName { get; }

        public ObservableGroupToken(int[] withComponents, int[] withoutComponents, string collectionName)
        {
            LookupGroup = new LookupGroup(withComponents, withoutComponents);
            CollectionName = collectionName;
        }
        
        public ObservableGroupToken(ILookupGroup lookupGroup, string collectionName)
        {
            LookupGroup = lookupGroup;
            CollectionName = collectionName;
        }

        public override int GetHashCode()
        {
            var requiredHash = LookupGroup.RequiredComponents?.GetHashCode() ?? 0;
            var excludedHash = LookupGroup.ExcludedComponents?.GetHashCode() ?? 0;
            var poolHash = CollectionName?.GetHashCode() ?? 0;
            return requiredHash ^ excludedHash ^ poolHash;
        }
    }
}