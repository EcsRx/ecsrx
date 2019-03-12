using System;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class EmptyGroup : IGroup
    {
        public Type[] RequiredComponents { get; } = new Type[0];
        public Type[] ExcludedComponents { get; } = new Type[0];
        public Predicate<IEntity> TargettedEntities => null;
    }
}