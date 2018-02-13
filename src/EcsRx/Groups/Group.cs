using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class Group : IGroup, IHasPredicate
    {
        public IEnumerable<Type> MatchesComponents { get; }
		public Predicate<IEntity> TargettedEntities { get; }
        
		public Group(params Type[] targettedComponents) : this(null, targettedComponents) {}

        public Group(Predicate<IEntity> targettedEntities, params Type[] matchesComponents)
        {
			TargettedEntities = targettedEntities;
            MatchesComponents = matchesComponents;
        }

		public bool CanProcessEntity (IEntity entity) {
			if(TargettedEntities == null) return true;
			return TargettedEntities(entity);
		}
    }
}