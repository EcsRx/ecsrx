using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Attributes;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEntity> MatchingGroup(this IEnumerable<IEntity> entities, IGroup group)
        { return entities.Where(group.Matches); }
        
        public static IEnumerable<IEntity> MatchingGroup(this IEnumerable<IEntity> entities, LookupGroup group)
        { return entities.Where(x => group.Matches(x)); }

        public static IEnumerable<IGroupSystem> GetApplicableSystems(this IEnumerable<IGroupSystem> systems, IEntity entity)
        { return systems.Where(x => entity.MatchesGroup(x.Group)); }

        public static IEnumerable<IGroupSystem> GetApplicableSystems(this IEnumerable<IGroupSystem> systems, IEnumerable<IComponent> components)
        {
            var componentTypes = components.Select(x => x.GetType());
            return systems.Where(x => x.Group.RequiredComponents.All(y => componentTypes.Contains(y)));
        }
    }
}