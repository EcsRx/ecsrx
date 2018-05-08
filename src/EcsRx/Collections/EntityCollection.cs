using System;
using System.Collections;
using System.Collections.Generic;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Exceptions;

namespace EcsRx.Collections
{
    public class EntityCollection : IEntityCollection
    {
        private readonly IDictionary<Guid, IEntity> _entities;

        public string Name { get; }
        public IEventSystem EventSystem { get; }
        public IEntityFactory EntityFactory { get; }

        public EntityCollection(string name, IEntityFactory entityFactory, IEventSystem eventSystem)
        {
            _entities = new Dictionary<Guid, IEntity>();
            Name = name;
            EventSystem = eventSystem;
            EntityFactory = entityFactory;
        }

        public IEntity CreateEntity(IBlueprint blueprint = null)
        {
            var entity = EntityFactory.Create(null);

            EventSystem.Publish(new EntityBeforeAddedEvent(entity, this));

            _entities.Add(entity.Id, entity);
            blueprint?.Apply(entity);

            EventSystem.Publish(new EntityAddedEvent(entity, this));

            return entity;
        }

        public void RemoveEntity(IEntity entity)
        {
            EventSystem.Publish(new EntityBeforeRemovedEvent(entity, this));

            _entities.Remove(entity.Id);
            entity.Dispose();

            EventSystem.Publish(new EntityRemovedEvent(entity, this));
        }

        public void AddEntity(IEntity entity)
        {
            if(entity.Id == Guid.Empty)
            { throw new InvalidEntityException("Entity provided does not have an assigned Id"); }

            EventSystem.Publish(new EntityBeforeAddedEvent(entity, this));
            _entities.Add(entity.Id, entity);
            EventSystem.Publish(new EntityAddedEvent(entity, this));
        }

        public bool ContainsEntity(IEntity entity)
        { return _entities.ContainsKey(entity.Id); }

        public IEnumerator<IEntity> GetEnumerator()
        { return _entities.Values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}
