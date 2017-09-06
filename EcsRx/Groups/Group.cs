using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups
{
    public class Group : IGroup, IHasPredicate
    {
        public IEnumerable<Type> TargettedComponents { get; private set; }
		public Predicate<IEntity> TargettedEntities { get; private set; }
        
		public Group(params Type[] targettedComponents) : this(null, targettedComponents) {}

        public Group(Predicate<IEntity> targettedEntities, params Type[] targettedComponents)
        {
			TargettedEntities = targettedEntities;
            TargettedComponents = targettedComponents;
        }

		public bool CanProcessEntity (IEntity entity) {
			if(TargettedEntities == null) return true;
			return TargettedEntities(entity);
		}
    }
}