using System;
using EcsRx.Entities;
using EcsRx.Groups.Observable.Tracking.Types;

namespace EcsRx.Groups.Observable.Tracking.Events
{
    public readonly struct GroupStateChanged : IEquatable<GroupStateChanged>
    {
        public readonly IEntity Entity;
        public readonly GroupActionType GroupActionType;

        public GroupStateChanged(IEntity entity, GroupActionType groupActionType)
        {
            Entity = entity;
            GroupActionType = groupActionType;
        }

        public bool Equals(GroupStateChanged other)
        {
            return Equals(Entity, other.Entity) && GroupActionType == other.GroupActionType;
        }

        public override bool Equals(object obj)
        {
            return obj is GroupStateChanged other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Entity != null ? Entity.GetHashCode() : 0) * 397) ^ (int)GroupActionType;
            }
        }
    }
}