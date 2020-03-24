using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Blueprints;
using EcsRx.Collections;
using EcsRx.Collections.Entity;
using EcsRx.Components;
using EcsRx.Entities;

namespace EcsRx.Extensions
{
    public static class IEntityCollectionExtensions
    {
        public static void RemoveEntitiesContaining<T>(this IEntityCollection entityCollection)
            where T : class, IComponent
        {
            var entities = entityCollection
                .Where(entity => entity.HasComponent<T>())
                .ToArray();
            
            for(var i=0;i<entities.Length;i++)
            { entityCollection.RemoveEntity(entities[i].Id); }
        }

        public static void RemoveEntitiesContaining(this IEntityCollection entityCollection, params Type[] components)
        {
            var entities = entityCollection
                .Where(entity => components.Any(x => entity.HasAllComponents(x)))
                .ToArray();

            for (var i = 0; i < entities.Length; i++)
            { entityCollection.RemoveEntity(entities[i].Id); }
        }

        public static void RemoveAllEntities(this IEntityCollection entityCollection)
        {
            var entities = entityCollection.ToArray();
            for (var i = 0; i < entities.Length; i++)
            { entityCollection.RemoveEntity(entities[i].Id); }
        }

        public static void RemoveEntities(this IEntityCollection entityCollection, Func<IEntity, bool> predicate)
        {
            var entities = entityCollection.Where(predicate).ToArray();
            for (var i = 0; i < entities.Length; i++)
            { entityCollection.RemoveEntity(entities[i].Id); }
        }

        public static void RemoveEntities(this IEntityCollection entityCollection, params IEntity[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            { entityCollection.RemoveEntity(entities[i].Id); }
        }

        public static void RemoveEntities(this IEntityCollection entityCollection, IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            { entityCollection.RemoveEntity(entity.Id); }
        }

        public static IEnumerable<IEntity> Query(this IEntityCollection entityCollection, IEntityCollectionQuery query)
        { return query.Execute(entityCollection); }

        public static IEntity CreateEntity(this IEntityCollection entityCollection, params IBlueprint[] blueprints)
        {
            var entity = entityCollection.CreateEntity();
            entity.ApplyBlueprints(blueprints);
            return entity;
        }

        public static IEntity CreateEntity(this IEntityCollection entityCollection, IEnumerable<IBlueprint> blueprints)
        {
            var entity = entityCollection.CreateEntity();
            entity.ApplyBlueprints(blueprints);
            return entity;
        }
    }
}