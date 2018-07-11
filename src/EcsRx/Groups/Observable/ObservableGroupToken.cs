using System;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupToken
    {
        public IGroup Group { get; }
        public string Pool { get; }

        public ObservableGroupToken(Type[] withComponents, Type[] withoutComponents, string pool)
        {
            Group = new Group(null, withComponents, withoutComponents);
            Pool = pool;
        }

        public ObservableGroupToken(IGroup group, string pool) : this(group.RequiredComponents, group.ExcludedComponents, pool) {}

        public override int GetHashCode()
        {
            var requiredHash = Group.RequiredComponents?.GetHashCode() ?? 0;
            var excludedHash = Group.ExcludedComponents?.GetHashCode() ?? 0;
            var poolHash = Pool?.GetHashCode() ?? 0;
            return requiredHash ^ excludedHash ^ poolHash;
        }
    }
}