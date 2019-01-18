using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class Group : IGroup, IHasPredicate
    {
        public Type[] RequiredComponents { get; }
	    public Type[] ExcludedComponents { get; }
		public Predicate<IEntity> EntityPredicate { get; }
        
		public Group(params Type[] requiredComponents) : this(null, requiredComponents) {}
        public Group(Predicate<IEntity> entityPredicate, params Type[] requiredComponents): this(entityPredicate, requiredComponents, new Type[0]){}

	    public Group(Predicate<IEntity> entityPredicate, IEnumerable<Type> requiredComponents, IEnumerable<Type> excludedComponents)
	    {
		    EntityPredicate = entityPredicate;
		    RequiredComponents = requiredComponents.ToArray();
		    ExcludedComponents = excludedComponents.ToArray();
	    }

		public bool CanProcessEntity (IEntity entity)
		{ return EntityPredicate == null || EntityPredicate(entity); }
    }
}