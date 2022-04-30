using System;
using System.Collections;
using System.Collections.Generic;
using SystemsRx.MicroRx.Subjects;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Lookups;

namespace EcsRx.Collections.Entity
{
    public class EntityCollection : IEntityCollection, IDisposable
    {
        public int Id { get; }
        public IEntityFactory EntityFactory { get; }
        
        public readonly EntityLookup EntityLookup;
        public IObservable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public IObservable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        
        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;
        
        public EntityCollection(int id, IEntityFactory entityFactory)
        {
            EntityLookup = new EntityLookup();
            Id = id;
            EntityFactory = entityFactory;

            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();
        }
        
        public IEntity CreateEntity(IBlueprint blueprint = null, int? id = null)
        {
            if (id.HasValue && EntityLookup.Contains(id.Value))
            { throw new InvalidOperationException("id already exists"); }

            var entity = EntityFactory.Create(id);

            EntityLookup.Add(entity);
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));

            blueprint?.Apply(entity);

            return entity;
        }

        public IEntity GetEntity(int id)
        { return EntityLookup[id]; }

        public void RemoveEntity(int id, bool disposeOnRemoval = true)
        {
            var entity = GetEntity(id);
            EntityLookup.Remove(id);

            var entityId = entity.Id;

            if (disposeOnRemoval)
            {
                entity.Dispose();
                EntityFactory.Destroy(entityId);
            }
            
            _onEntityRemoved.OnNext(new CollectionEntityEvent(entity));
        }

        public void AddEntity(IEntity entity)
        {
            EntityLookup.Add(entity);
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
        }

        public bool ContainsEntity(int id)
        { return EntityLookup.Contains(id); }

        public IEnumerator<IEntity> GetEnumerator()
        { return EntityLookup.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Dispose()
        {
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();

            EntityLookup.Clear();
        }

        public int Count => EntityLookup.Count;
        public IEntity this[int index] => EntityLookup.GetByIndex(index);
    }
}
