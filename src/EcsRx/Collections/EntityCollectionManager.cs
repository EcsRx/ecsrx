using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    public class EntityCollectionManager : IEntityCollectionManager, IDisposable
    {
        public const string DefaultPoolName = "default";
        
        private readonly IDictionary<ObservableGroupToken, IObservableGroup> _observableGroups;
        private readonly IDictionary<string, IEntityCollection> _collections;

        public IEventSystem EventSystem { get; }
        public IEnumerable<IEntityCollection> Collections => _collections.Values;
        public IEntityCollectionFactory EntityCollectionFactory { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }

        public EntityCollectionManager(IEventSystem eventSystem, IEntityCollectionFactory entityCollectionFactory, IObservableGroupFactory observableGroupFactory)
        {
            EventSystem = eventSystem;
            EntityCollectionFactory = entityCollectionFactory;
            ObservableGroupFactory = observableGroupFactory;

            _observableGroups = new Dictionary<ObservableGroupToken, IObservableGroup>();
            _collections = new Dictionary<string, IEntityCollection>();

            CreateCollection(DefaultPoolName);
        }
        
        public IEntityCollection CreateCollection(string name)
        {
            var pool = EntityCollectionFactory.Create(name);
            _collections.Add(name, pool);

            EventSystem.Publish(new PoolAddedEvent(pool));

            return pool;
        }

        public IEntityCollection GetCollection(string name = null)
        { return _collections[name ?? DefaultPoolName]; }

        public void RemoveCollection(string name, bool disposeEntities = true)
        {
            if(!_collections.ContainsKey(name)) { return; }

            var pool = _collections[name];
            _collections.Remove(name);

            EventSystem.Publish(new PoolRemovedEvent(collection));
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
            var observableGroupToken = new ObservableGroupToken(group.MatchesComponents.ToArray(), collectionName);
            if (_observableGroups.ContainsKey(observableGroupToken)) { return _observableGroups[observableGroupToken]; }

            var entityMatches = GetEntitiesFor(group, collectionName);
            var groupAccessor = ObservableGroupFactory.Create(new ObservableGroupConfiguration
            {
                ObservableGroupToken = observableGroupToken,
                InitialEntities = entityMatches
            });
            
            _observableGroups.Add(observableGroupToken, groupAccessor);

            return _observableGroups[observableGroupToken];
        }

        public void Dispose()
        {
            foreach (var accessor in _observableGroups.Values)
            { (accessor as IDisposable)?.Dispose(); }
        }
    }
}