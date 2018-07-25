using System;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupToken
    {
        public IGroup Group { get; }
        public string CollectionName { get; }

        public ObservableGroupToken(Type[] withComponents, Type[] withoutComponents, string collectionName)
        {
            Group = new Group(null, withComponents, withoutComponents);
            CollectionName = collectionName;
        }

        public ObservableGroupToken(IGroup group, string pool) : this(group.RequiredComponents, group.ExcludedComponents, pool) {}

        public override int GetHashCode()
        {
            var requiredHash = Group.RequiredComponents?.GetHashCode() ?? 0;
            var excludedHash = Group.ExcludedComponents?.GetHashCode() ?? 0;
            var poolHash = CollectionName?.GetHashCode() ?? 0;
            return requiredHash ^ excludedHash ^ poolHash;
        }
    }
}