using System;
using System.Collections.Generic;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Lookups;

namespace EcsRx.Collections.Database
{
    public class EntityDatabase : IEntityDatabase
    {
        private readonly CollectionLookup _collections;
        private readonly IDictionary<int, IDisposable> _collectionSubscriptions;

        public IReadOnlyList<IEntityCollection> Collections => _collections;

        public IEntityCollectionFactory EntityCollectionFactory { get; }
        
        public IObservable<CollectionEntityEvent> EntityAdded => _onEntityAdded;
        public IObservable<CollectionEntityEvent> EntityRemoved => _onEntityRemoved;
        public IObservable<ComponentsChangedEvent> EntityComponentsAdded => _onEntityComponentsAdded;
        public IObservable<ComponentsChangedEvent> EntityComponentsRemoving => _onEntityComponentsRemoving;
        public IObservable<ComponentsChangedEvent> EntityComponentsRemoved => _onEntityComponentsRemoved;
        public IObservable<IEntityCollection> CollectionAdded => _onCollectionAdded;
        public IObservable<IEntityCollection> CollectionRemoved => _onCollectionRemoved;

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
        {
            if(id == EntityCollectionLookups.DefaultCollectionId && _collections.Count == 0)
            { CreateCollection(EntityCollectionLookups.DefaultCollectionId); }
            
            return _collections[id];
        }

        public void RemoveCollection(int id, bool disposeEntities = true)
        {
            if(!_collections.Contains(id)) { return; }

            var collection = _collections[id];
            _collections.Remove(id);
            
            UnsubscribeFromCollection(id);

            _onCollectionRemoved.OnNext(collection);
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(IGroup group, int collectionId = EntityCollectionLookups.NoCollectionDefined)
        {
            if(group is EmptyGroup)
            { return Array.Empty<IEntity>(); }

            if (collectionId != EntityCollectionLookups.NoCollectionDefined)
            { return _collections[collectionId].MatchingGroup(group); }

            return Collections.GetAllEntities().MatchingGroup(group);
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(IGroup group, params int[] collectionIds)
        {
            if(group is EmptyGroup)
            { return Array.Empty<IEntity>(); }

            if (collectionIds == null || collectionIds.Length == 0)
            { return Collections.GetAllEntities().MatchingGroup(group); }

            var matchingEntities = new List<IEntity>();
            foreach (var collectionName in collectionIds)
            {
                var results = _collections[collectionName].MatchingGroup(group);
                matchingEntities.AddRange(results);
            }

            return matchingEntities;
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(LookupGroup lookupGroup, params int[] collectionIds)
        {
            if(lookupGroup.RequiredComponents.Length == 0 && lookupGroup.ExcludedComponents.Length  == 0)
            { return Array.Empty<IEntity>(); }

            if (collectionIds == null || collectionIds.Length == 0)
            { return Collections.GetAllEntities().MatchingGroup(lookupGroup); }

            var matchingEntities = new List<IEntity>();
            foreach (var collectionName in collectionIds)
            {
                var results = _collections[collectionName].MatchingGroup(lookupGroup);
                matchingEntities.AddRange(results);
            }

            return matchingEntities;
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