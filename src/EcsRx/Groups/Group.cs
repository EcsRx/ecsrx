using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class Group : IGroup, IHasPredicate, IWithoutComponents
    {
        public IEnumerable<Type> WithComponents { get; }
	    public IEnumerable<Type> WithoutComponents { get; }
		public Predicate<IEntity> EntityPredicate { get; }
        
		public Group(params Type[] targettedComponents) : this(null, targettedComponents) {}
        public Group(Predicate<IEntity> entityPredicate, params Type[] withComponents): this(entityPredicate, withComponents, new Type[0]){}

	    public Group(Predicate<IEntity> entityPredicate, IEnumerable<Type> withComponents, IEnumerable<Type> withoutComponents)
	    {
		    EntityPredicate = entityPredicate;
		    WithComponents = withComponents;
		    WithoutComponents = withoutComponents;
	    }

		public bool CanProcessEntity (IEntity entity)
		{ return EntityPredicate == null || EntityPredicate(entity); }	    
    }
}