using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class EmptyGroup : IGroup
    {
        public IEnumerable<Type> WithComponents => new Type[0];
        public Predicate<IEntity> TargettedEntities => null;
    }
}