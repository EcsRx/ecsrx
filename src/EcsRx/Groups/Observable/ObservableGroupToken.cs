using System;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupToken
    {
        public ILookupGroup LookupGroup { get; }
        public string[] CollectionNames { get; }

        public ObservableGroupToken(int[] withComponents, int[] withoutComponents, params string[] collectionNames)
        {
            LookupGroup = new LookupGroup(withComponents, withoutComponents);
            CollectionNames = collectionNames;
        }
        
        public ObservableGroupToken(ILookupGroup lookupGroup, params string[] collectionNames)
        {
            LookupGroup = lookupGroup;
            CollectionNames = collectionNames;
        }

        public override int GetHashCode()
        {
            var requiredHash = LookupGroup.RequiredComponents?.GetHashCode() ?? 0;
            var excludedHash = LookupGroup.ExcludedComponents?.GetHashCode() ?? 0;
            var poolHash = CollectionNames?.GetHashCode() ?? 0;
            return requiredHash ^ excludedHash ^ poolHash;
        }
    }
}