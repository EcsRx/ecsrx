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
            return Observable.Timer(TimeSpan.FromSeconds(1))
                .Where(x => predicate(entity))
                .Select(x => entity);
        }

        public static bool MatchesGroup(this IEntity entity, IGroup group)
        { return entity.HasComponents(group.TargettedComponents.ToArray()); }

        public static IEntity ApplyBlueprint(this IEntity entity, IBlueprint blueprint)
        {
            blueprint.Apply(entity);
            return entity;
        }

        public static IEntity ApplyBlueprints(this IEntity entity, params IBlueprint[] blueprints)
        {
            blueprints.ForEachRun(x => x.Apply(entity));
            return entity;
        }

        public static IEntity ApplyBlueprints(this IEntity entity, IEnumerable<IBlueprint> blueprints)
        {
            blueprints.ForEachRun(x => x.Apply(entity));
            return entity;
        }

        public static void RemoveComponents(this IEntity entity, Func<IComponent, bool> predicate)
        {
            var matchingComponents = entity.Components.Where(predicate).ToArray();
            matchingComponents.ForEachRun(entity.RemoveComponent);
        }

        public static void RemoveComponents(this IEntity entity, params IComponent[] components)
        { components.ForEachRun(entity.RemoveComponent); }

        public static void RemoveComponents(this IEntity entity, IEnumerable<IComponent> components)
        { components.ForEachRun(entity.RemoveComponent); }
    }
}