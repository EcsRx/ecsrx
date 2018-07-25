using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Polyfills;

namespace EcsRx.Collections
{
    public class EntityCollectionManager : IEntityCollectionManager, IDisposable
    {
        public const string DefaultPoolName = "default";
        
        private readonly IDictionary<ObservableGroupToken, IObservableGroup> _observableGroups;
        private readonly IDictionary<string, IEntityCollection> _collections;
        private readonly IDictionary<string, IDisposable> _collectionSubscriptions;

        public IEnumerable<IEntityCollection> Collections => _collections.Values;
        public IEntityCollectionFactory EntityCollectionFactory { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }
        
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

        public EntityCollectionManager(IEntityCollectionFactory entityCollectionFactory, IObservableGroupFactory observableGroupFactory)
        {
            EntityCollectionFactory = entityCollectionFactory;
            ObservableGroupFactory = observableGroupFactory;

            _observableGroups = new Dictionary<ObservableGroupToken, IObservableGroup>();
            _collections = new Dictionary<string, IEntityCollection>();
            _collectionSubscriptions = new Dictionary<string, IDisposable>();
            
            _onEntityAdded = new Subject<CollectionEntityEvent>();
            _onEntityRemoved = new Subject<CollectionEntityEvent>();
            _onEntityComponentsAdded = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoving = new Subject<ComponentsChangedEvent>();
            _onEntityComponentsRemoved = new Subject<ComponentsChangedEvent>();

            CreateCollection(DefaultPoolName);
        }

        public void SubscribeToCollection(IEntityCollection collection)
        {
            var collectionDisposable = new CompositeDisposable();

            collection.EntityAdded.Subscribe(x => _onEntityAdded.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityRemoved.Subscribe(x => _onEntityRemoved.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityComponentsAdded.Subscribe(x => _onEntityComponentsAdded.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityComponentsRemoving.Subscribe(x => _onEntityComponentsRemoving.OnNext(x)).AddTo(collectionDisposable);
            collection.EntityComponentsRemoved.Subscribe(x => _onEntityComponentsRemoved.OnNext(x)).AddTo(collectionDisposable);
            
            _collectionSubscriptions.Add(collection.Name, collectionDisposable);
        }

        public void UnsubscribeFromCollection(string collectionName)
        { _collectionSubscriptions.RemoveAndDispose(collectionName); }
        
        public IEntityCollection CreateCollection(string name)
        {
            var collection = EntityCollectionFactory.Create(name);
            _collections.Add(name, collection);
            SubscribeToCollection(collection);

            //EventSystem.Publish(new CollectionAddedEvent(collection));

            return collection;
        }

        public IEntityCollection GetCollection(string name = null)
        { return _collections[name ?? DefaultPoolName]; }

        public void RemoveCollection(string name, bool disposeEntities = true)
        {
            if(!_collections.ContainsKey(name)) { return; }

            var collection = _collections[name];
            _collections.Remove(name);
            
            UnsubscribeFromCollection(name);

            //EventSystem.Publish(new CollectionRemovedEvent(collection));
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(IGroup group, string collectionName = null)
        {
            if(group is EmptyGroup)
            { return new IEntity[0]; }

            if (collectionName != null)
            { return _collections[collectionName].MatchingGroup(group); }

            return Collections.GetAllEntities().MatchingGroup(group);
        }

        public IObservableGroup GetObservableGroup(IGroup group, string collectionName = null)
        {
            var observableGroupToken = new ObservableGroupToken(group, collectionName);
            if (_observableGroups.ContainsKey(observableGroupToken)) { return _observableGroups[observableGroupToken]; }

            var entityMatches = GetEntitiesFor(group, collectionName);
            var configuration = new ObservableGroupConfiguration
            {
                ObservableGroupToken = observableGroupToken,
                InitialEntities = entityMatches
            };

            if (collectionName != null)
            { configuration.NotifyingCollection = _collections[collectionName]; }
            else
            { configuration.NotifyingCollection = this; }
            
            var observableGroup = ObservableGroupFactory.Create(configuration);
            _observableGroups.Add(observableGroupToken, observableGroup);

            return _observableGroups[observableGroupToken];
        }

        public void Dispose()
        {
            foreach (var observableGroup in _observableGroups.Values)
            { (observableGroup as IDisposable)?.Dispose(); }
            
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
            _onEntityComponentsAdded.Dispose();
            _onEntityComponentsRemoving.Dispose();
            _onEntityComponentsRemoved.Dispose();
            
            _collectionSubscriptions.RemoveAndDisposeAll();
        }
    }
}