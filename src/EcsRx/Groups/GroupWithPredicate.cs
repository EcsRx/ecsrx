using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class GroupWithPredicate : Group, IHasPredicate
    {
        public Predicate<IEntity> EntityPredicate { get; }
        
        public GroupWithPredicate(Predicate<IEntity> entityPredicate, params Type[] requiredComponents): this(entityPredicate, requiredComponents, Array.Empty<Type>()){}

        public GroupWithPredicate(Predicate<IEntity> entityPredicate, IEnumerable<Type> requiredComponents, IEnumerable<Type> excludedComponents) : base(requiredComponents, excludedComponents)
        {
            EntityPredicate = entityPredicate;
        }

        public bool CanProcessEntity (IEntity entity)
        { return EntityPredicate == null || EntityPredicate(entity); }
    }
}