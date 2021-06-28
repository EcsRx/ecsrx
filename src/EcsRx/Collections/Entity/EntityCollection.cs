using System;
using System.Collections;
using System.Collections.Generic;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
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
        public readonly IDictionary<int, IDisposable> EntitySubscriptions;

        public IObservable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public IObservable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        public IObservable<ComponentsChangedEvent> EntityComponentsAdded => _onEntityComponentsAdded;
        public IObservable<ComponentsChangedEvent> EntityComponentsRemoving => _onEntityComponentsRemoving;
        public IObservable<ComponentsChangedEvent> EntityComponentsRemoved => _onEntityComponentsRemoved;
        
        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsAdded;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoving;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoved;
        
        public EntityCollection(int id, IEntityFactory entityFactory)
        {
            EntityLookup = new EntityLookup();
            EntitySubscriptions = new Dictionary<int, IDisposable>();
            Id = id;
            EntityFactory = entityFactory;

            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();
            _onEntityComponentsAdded = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoving = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoved = new Subject<ComponentsChangedEvent>();
        }

        public void SubscribeToEntity(IEntity entity)
        {
            var entityDisposable = new CompositeDisposable();
            entity.ComponentsAdded.Subscribe(x => _onEntityComponentsAdded.OnNext(new ComponentsChangedEvent(entity, x))).AddTo(entityDisposable);
            entity.ComponentsRemoving.Subscribe(x => _onEntityComponentsRemoving.OnNext(new ComponentsChangedEvent(entity, x))).AddTo(entityDisposable);
            entity.ComponentsRemoved.Subscribe(x => _onEntityComponentsRemoved.OnNext(new ComponentsChangedEvent(entity, x))).AddTo(entityDisposable);
            EntitySubscriptions.Add(entity.Id, entityDisposable);
        }

        public void UnsubscribeFromEntity(int entityId)
        { EntitySubscriptions.RemoveAndDispose(entityId); }
        
        public IEntity CreateEntity(IBlueprint blueprint = null, int? id = null)
        {
            if (id.HasValue && EntityLookup.Contains(id.Value))
            { throw new InvalidOperationException("id already exists"); }

            var entity = EntityFactory.Create(id);

            EntityLookup.Add(entity);
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
            SubscribeToEntity(entity);
           
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
            
            UnsubscribeFromEntity(entityId);
            
            _onEntityRemoved.OnNext(new CollectionEntityEvent(entity));
        }

        public void AddEntity(IEntity entity)
        {
            EntityLookup.Add(entity);
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity));
            SubscribeToEntity(entity);
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
            _onEntityComponentsAdded.Dispose();
            _onEntityComponentsRemoving.Dispose();
            _onEntityComponentsRemoved.Dispose();

            EntityLookup.Clear();
            EntitySubscriptions.RemoveAndDisposeAll();
        }

        public int Count => EntityLookup.Count;
        public IEntity this[int index] => EntityLookup.GetByIndex(index);
    }
}
