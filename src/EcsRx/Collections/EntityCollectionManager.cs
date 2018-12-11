using System;
using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Collections
{
    public class EntityCollectionManager : IEntityCollectionManager, IDisposable
    {
        public const string DefaultPoolName = "default";
        
        private readonly IDictionary<ObservableGroupToken, IObservableGroup> _observableGroups;
        private readonly IDictionary<string, IEntityCollection> _collections;
        private readonly IDictionary<string, IDisposable> _collectionSubscriptions;

        public IEnumerable<IEntityCollection> Collections => _collections.Values;
        public IEnumerable<IObservableGroup> ObservableGroups => _observableGroups.Values;

        public IEntityCollectionFactory EntityCollectionFactory { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
       public IObservable<IEntityCollection> CollectionAdded => _onCollectionAdded;
        public IObservable<IEntityCollection> CollectionRemoved => _onCollectionRemoved;
        private readonly Subject<IEntityCollection> _onCollectionAdded;
        private readonly Subject<IEntityCollection> _onCollectionRemoved;

        public EntityCollectionManager(IEntityCollectionFactory entityCollectionFactory, IObservableGroupFactory observableGroupFactory, IComponentTypeLookup componentTypeLookup)
        {
            EntityCollectionFactory = entityCollectionFactory;
            ObservableGroupFactory = observableGroupFactory;
            ComponentTypeLookup = componentTypeLookup;

            _observableGroups = new Dictionary<ObservableGroupToken, IObservableGroup>();
            _collections = new Dictionary<string, IEntityCollection>();
            _collectionSubscriptions = new Dictionary<string, IDisposable>();
            _onCollectionAdded = new Subject<IEntityCollection>();
            _onCollectionRemoved = new Subject<IEntityCollection>();

            CreateCollection(DefaultPoolName);
        }

        public void SubscribeToCollection(IEntityCollection collection)
        {
            var collectionDisposable = new CompositeDisposable();           
            _collectionSubscriptions.Add(collection.Name, collectionDisposable);
        }

        public void UnsubscribeFromCollection(string collectionName)
        { _collectionSubscriptions.RemoveAndDispose(collectionName); }
        
        public IEntityCollection CreateCollection(string name)
        {
            var collection = EntityCollectionFactory.Create(name);
            _collections.Add(name, collection);
            SubscribeToCollection(collection);

            _onCollectionAdded.OnNext(collection);
            
            return collection;
        }
        
        public IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds)
        {
            foreach(var groupLookup in _observableGroups)
            {
                if (groupLookup.Key.LookupGroup.Matches(componentTypeIds))
                { yield return groupLookup.Value; }
            }
        }

        public IEntityCollection GetCollection(string name = null)
        { return _collections[name ?? DefaultPoolName]; }

        public void RemoveCollection(string name, bool disposeEntities = true)
        {
            if(!_collections.ContainsKey(name)) { return; }

            var collection = _collections[name];
            _collections.Remove(name);
            
            UnsubscribeFromCollection(name);

            _onCollectionRemoved.OnNext(collection);
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(IGroup group, string collectionName = null)
        {
            if(group is EmptyGroup)
            { return new IEntity[0]; }

            if (collectionName != null)
            { return _collections[collectionName].MatchingGroup(group); }

            return Collections.GetAllEntities().MatchingGroup(group);
        }
        
        public IEnumerable<IEntity> GetEntitiesFor(ILookupGroup lookupGroup, string collectionName = null)
        {
            if(lookupGroup.RequiredComponents.Length == 0 && lookupGroup.ExcludedComponents.Length  == 0)
            { return new IEntity[0]; }

            if (collectionName != null)
            { return _collections[collectionName].MatchingGroup(lookupGroup); }

            return Collections.GetAllEntities().MatchingGroup(lookupGroup);
        }

        public IObservableGroup GetObservableGroup(IGroup group, string collectionName = null)
        {
            var requiredComponents = ComponentTypeLookup.GetComponentTypes(group.RequiredComponents);
            var excludedComponents = ComponentTypeLookup.GetComponentTypes(group.ExcludedComponents);
            var lookupGroup = new LookupGroup(requiredComponents, excludedComponents);
            var observableGroupToken = new ObservableGroupToken(lookupGroup, collectionName);
            if (_observableGroups.ContainsKey(observableGroupToken)) { return _observableGroups[observableGroupToken]; }

            var entityMatches = GetEntitiesFor(lookupGroup, collectionName);
            var configuration = new ObservableGroupConfiguration
            {
                ObservableGroupToken = observableGroupToken,
                InitialEntities = entityMatches
            };

            configuration.NotifyingCollection = _collections[collectionName ?? DefaultPoolName];
            
            var observableGroup = ObservableGroupFactory.Create(configuration);
            _observableGroups.Add(observableGroupToken, observableGroup);

            return _observableGroups[observableGroupToken];
        }

        public void Dispose()
        {
            foreach (var observableGroup in _observableGroups.Values)
            { (observableGroup as IDisposable)?.Dispose(); }
            
            _collectionSubscriptions.RemoveAndDisposeAll();
        }
    }
}