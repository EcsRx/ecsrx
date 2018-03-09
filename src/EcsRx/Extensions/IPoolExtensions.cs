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
            var entities = pool.Entities
                .Where(entity => entity.HasComponent<T>())
                .ToArray();
            
            for(var i=0;i<entities.Length;i++)
            { pool.RemoveEntity(entities[i]); }
        }

        public static void RemoveEntitiesContaining(this IPool pool, params Type[] components)
        {
            var entities = pool.Entities
                .Where(entity => components.Any(x => entity.HasComponents(x)))
                .ToArray();

            for (var i = 0; i < entities.Length; i++)
            { pool.RemoveEntity(entities[i]); }
        }

        public static void RemoveAllEntities(this IPool pool)
        {
            var entities = pool.Entities.ToArray();
            for (var i = 0; i < entities.Length; i++)
            { pool.RemoveEntity(entities[i]); }
        }

        public static void RemoveEntities(this IPool pool, Func<IEntity, bool> predicate)
        {
            var entities = pool.Entities.Where(predicate).ToArray();
            for (var i = 0; i < entities.Length; i++)
            { pool.RemoveEntity(entities[i]); }
        }

        public static void RemoveEntities(this IPool pool, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { pool.RemoveEntity(entities[i]); }
        }

        public static void RemoveEntities(this IPool pool, IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            { pool.RemoveEntity(entity); }
        }

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