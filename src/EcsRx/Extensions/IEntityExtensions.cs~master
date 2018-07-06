using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Groups;

namespace EcsRx.Extensions
{
    public static class IEntityExtensions
    {
        public static IObservable<IEntity> WaitForPredicateMet(this IEntity entity, Predicate<IEntity> predicate)
        {
            return Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(x => predicate(entity))
                .Select(x => entity);
        }

        public static bool MatchesGroup(this IEntity entity, IGroup group)
        { return entity.HasComponents(group.MatchesComponents.ToArray()); }

        public static IEntity ApplyBlueprint(this IEntity entity, IBlueprint blueprint)
        {
            blueprint.Apply(entity);
            return entity;
        }

        public static IEntity ApplyBlueprints(this IEntity entity, params IBlueprint[] blueprints)
        {
            for (var i = 0; i < blueprints.Length; i++)
            { blueprints[i].Apply(entity); }

            return entity;
        }

        public static IEntity ApplyBlueprints(this IEntity entity, IEnumerable<IBlueprint> blueprints)
        {
            foreach (var blueprint in blueprints)
            { blueprint.Apply(entity); }

            return entity;
        }
    }
}