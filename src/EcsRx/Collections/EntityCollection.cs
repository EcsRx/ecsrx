using System;
using System.Collections;
using System.Collections.Generic;
using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Exceptions;
using EcsRx.Extensions;
using EcsRx.MicroRx;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Collections
{
    public class EntityCollection : IEntityCollection, IDisposable
    {
        public readonly IDictionary<int, IEntity> EntityLookup;
        public readonly IDictionary<int, IDisposable> EntitySubscriptions;

        public IObservable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public IObservable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        public IObservable<ComponentsChangedEvent> EntityComponentsAdded => _onEntityComponentsAdded;
        public IObservable<ComponentsChangedEvent> EntityComponentsRemoving => _onEntityComponentsRemoving;
        public IObservable<ComponentsChangedEvent> EntityComponentsRemoved => _onEntityComponentsRemoved;
        
        public string Name { get; }
        public IEntityFactory EntityFactory { get; }

        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsAdded;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoving;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoved;
        
        public EntityCollection(string name, IEntityFactory entityFactory)
        {
            EntityLookup = new Dictionary<int, IEntity>();
            EntitySubscriptions = new Dictionary<int, IDisposable>();
            Name = name;
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
            entity.ComponentsAdded.Subscribe(x => _onEntityComponentsAdded.OnNext(new ComponentsChangedEvent(this, entity, x))).AddTo(entityDisposable);
            entity.ComponentsRemoving.Subscribe(x => _onEntityComponentsRemoving.OnNext(new ComponentsChangedEvent(this, entity, x))).AddTo(entityDisposable);
            entity.ComponentsRemoved.Subscribe(x => _onEntityComponentsRemoved.OnNext(new ComponentsChangedEvent(this, entity, x))).AddTo(entityDisposable);
            EntitySubscriptions.Add(entity.Id, entityDisposable);
        }

        public void UnsubscribeFromEntity(int entityId)
        { EntitySubscriptions.RemoveAndDispose(entityId); }
        
        public IEntity CreateEntity(IBlueprint blueprint = null)
        {
            var entity = EntityFactory.Create(null);

            EntityLookup.Add(entity.Id, entity);
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity, this));
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
            
            _onEntityRemoved.OnNext(new CollectionEntityEvent(entity, this));
        }

        public void AddEntity(IEntity entity)
        {
            EntityLookup.Add(entity.Id, entity);
            _onEntityAdded.OnNext(new CollectionEntityEvent(entity, this));
            SubscribeToEntity(entity);
        }

        public bool ContainsEntity(int id)
        { return EntityLookup.ContainsKey(id); }

        public IEnumerator<IEntity> GetEnumerator()
        { return EntityLookup.Values.GetEnumerator(); }

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
    }
}
