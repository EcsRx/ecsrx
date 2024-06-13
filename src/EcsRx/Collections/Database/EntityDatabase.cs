using System;
using System.Collections.Generic;
using SystemsRx.Extensions;
using EcsRx.Collections.Entity;
using EcsRx.Collections.Events;
using EcsRx.Lookups;
using R3;

namespace EcsRx.Collections.Database
{
    public class EntityDatabase : IEntityDatabase
    {
        private readonly CollectionLookup _collections;
        private readonly IDictionary<int, IDisposable> _collectionSubscriptions;

        public IReadOnlyList<IEntityCollection> Collections => _collections;
        public IEntityCollection this[int id] => _collections[id];

        public IEntityCollectionFactory EntityCollectionFactory { get; }
        
        public Observable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public Observable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        public Observable<ComponentsChangedEvent> EntityComponentsAdded => _onEntityComponentsAdded;
        public Observable<ComponentsChangedEvent> EntityComponentsRemoving => _onEntityComponentsRemoving;
        public Observable<ComponentsChangedEvent> EntityComponentsRemoved => _onEntityComponentsRemoved;
        public Observable<IEntityCollection> CollectionAdded => _onCollectionAdded;
        public Observable<IEntityCollection> CollectionRemoved => _onCollectionRemoved;

        private readonly Subject<IEntityCollection> _onCollectionAdded;
        private readonly Subject<IEntityCollection> _onCollectionRemoved;
        private readonly Subject<CollectionEntityEvent> _onEntityAdded;
        private readonly Subject<CollectionEntityEvent> _onEntityRemoved;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsAdded;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoving;
        private readonly Subject<ComponentsChangedEvent> _onEntityComponentsRemoved;

        public EntityDatabase(IEntityCollectionFactory entityCollectionFactory)
        {
            EntityCollectionFactory = entityCollectionFactory;

            _collections = new CollectionLookup();
            _collectionSubscriptions = new Dictionary<int, IDisposable>();
            _onCollectionAdded = new Subject<IEntityCollection>();
            _onCollectionRemoved = new Subject<IEntityCollection>();
            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();
            _onEntityComponentsAdded = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoving = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoved = new Subject<ComponentsChangedEvent>();

            CreateCollection(EntityCollectionLookups.DefaultCollectionId);
        }

        public void SubscribeToCollection(IEntityCollection collection)
        {
            var collectionDisposable = new CompositeDisposable();   
            collection.EntityAdded.Subscribe(x => _onEntityAdded.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityRemoved.Subscribe(x => _onEntityRemoved.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityComponentsAdded.Subscribe(x => _onEntityComponentsAdded.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityComponentsRemoving.Subscribe(x => _onEntityComponentsRemoving.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityComponentsRemoved.Subscribe(x => _onEntityComponentsRemoved.OnNext(x)).AddTo(collectionDisposable);

            _collectionSubscriptions.Add(collection.Id, collectionDisposable);
        }

        public void UnsubscribeFromCollection(int id)
        { _collectionSubscriptions.RemoveAndDispose(id); }
        
        public IEntityCollection CreateCollection(int id)
        {
            var collection = EntityCollectionFactory.Create(id);
            AddCollection(collection);
            return collection;
        }
        
        public void AddCollection(IEntityCollection collection)
        {
            _collections.Add(collection);
            SubscribeToCollection(collection);

            _onCollectionAdded.OnNext(collection);
        }

        public IEntityCollection GetCollection(int id = EntityCollectionLookups.DefaultCollectionId)
        { return _collections.Contains(id) ? _collections[id] : null; }

        public void RemoveCollection(int id, bool disposeEntities = true)
        {
            if(!_collections.Contains(id)) { return; }

            var collection = _collections[id];
            _collections.Remove(id);
            
            UnsubscribeFromCollection(id);

            _onCollectionRemoved.OnNext(collection);
        }

        public void Dispose()
        {
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
            _onEntityComponentsAdded.Dispose();
            _onEntityComponentsRemoving.Dispose();
            _onEntityComponentsRemoved.Dispose();
            
            _collectionSubscriptions.RemoveAndDisposeAll();
        }
    }
}