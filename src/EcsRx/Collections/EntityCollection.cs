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
        public readonly IDictionary<Guid, IEntity> EntityLookup;

        public string Name { get; }
        public IEventSystem EventSystem { get; }
        public IEntityFactory EntityFactory { get; }

        public EntityCollection(string name, IEntityFactory entityFactory, IEventSystem eventSystem)
        {
            EntityLookup = new Dictionary<Guid, IEntity>();
            Name = name;
            EventSystem = eventSystem;
            EntityFactory = entityFactory;
        }

        public IEntity CreateEntity(IBlueprint blueprint = null)
        {
            var entity = EntityFactory.Create(null);

            EventSystem.Publish(new EntityBeforeAddedEvent(entity, this));

            EntityLookup.Add(entity.Id, entity);
            blueprint?.Apply(entity);

            EventSystem.Publish(new EntityAddedEvent(entity, this));

            return entity;
        }

        public IEntity GetEntity(Guid id)
        { return EntityLookup[id]; }

        public void RemoveEntity(Guid id, bool disposeOnRemoval = true)
        {
            var entity = GetEntity(id);
            EventSystem.Publish(new EntityBeforeRemovedEvent(entity, this));
            EntityLookup.Remove(id);
            
            if(disposeOnRemoval)
            { entity.Dispose(); }

            EventSystem.Publish(new EntityRemovedEvent(entity, this));
        }

        public void AddEntity(IEntity entity)
        {
            if(entity.Id == Guid.Empty)
            { throw new InvalidEntityException("Entity provided does not have an assigned Id"); }

            EventSystem.Publish(new EntityBeforeAddedEvent(entity, this));
            EntityLookup.Add(entity.Id, entity);
            EventSystem.Publish(new EntityAddedEvent(entity, this));
        }

        public bool ContainsEntity(Guid id)
        { return EntityLookup.ContainsKey(id); }

        public IEnumerator<IEntity> GetEnumerator()
        { return EntityLookup.Values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}
