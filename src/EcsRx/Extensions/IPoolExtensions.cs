using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Extensions
{
    public static class IPoolExtensions
    {
        public static void RemoveEntitiesContaining<T>(this IPool pool)
            where T : class, IComponent
        {
            pool.Entities.Where(entity => entity.HasComponent<T>())
                .ToArray()
                .ForEachRun(pool.RemoveEntity);
        }

        public static void RemoveEntitiesContaining(this IPool pool, params Type[] components)
        {
            pool.Entities.Where(entity => components.Any(x => entity.HasComponents(x)))
                .ToArray()
                .ForEachRun(pool.RemoveEntity);
        }

        public static void RemoveAllEntities(this IPool pool)
        {
            var allEntities = pool.Entities.ToArray();
            allEntities.ForEachRun(pool.RemoveEntity);
        }

        public static void RemoveEntities(this IPool pool, Func<IEntity, bool> predicate)
        {
            var applicableEntities = pool.Entities.Where(predicate).ToArray();
            applicableEntities.ForEachRun(pool.RemoveEntity);
        }

        public static void RemoveEntities(this IPool pool, params IEntity[] entities)
        { entities.ForEachRun(pool.RemoveEntity); }

        public static void RemoveEntities(this IPool pool, IEnumerable<IEntity> entities)
        { entities.ForEachRun(pool.RemoveEntity); }

        public static IEnumerable<IEntity> Query(this IPool pool, IPoolQuery query)
        { return query.Execute(pool.Entities); }

        public static IEntity CreateEntity(this IPool pool, params IBlueprint[] blueprints)
        {
            var entity = pool.CreateEntity();
            entity.ApplyBlueprints(blueprints);
            return entity;
        }

        public static IEntity CreateEntity(this IPool pool, IEnumerable<IBlueprint> blueprints)
        {
            var entity = pool.CreateEntity();
            entity.ApplyBlueprints(blueprints);
            return entity;
        }
    }
}