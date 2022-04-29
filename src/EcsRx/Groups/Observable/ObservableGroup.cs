using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemsRx.Extensions;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking;
using EcsRx.Lookups;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroup : IObservableGroup
    {
        public readonly EntityLookup CachedEntities;
        public readonly List<IDisposable> Subscriptions;
        public readonly Dictionary<int, IObservableGroupTracker> GroupTrackers;

        public IObservable<IEntity> OnEntityAdded => _onEntityAdded;
        public IObservable<IEntity> OnEntityRemoved => _onEntityRemoved;
        public IObservable<IEntity> OnEntityRemoving => _onEntityRemoving;

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        private readonly Subject<IEntity> _onEntityRemoving;
        
        public ObservableGroupToken Token { get; }
        public IEnumerable<INotifyingEntityCollection> NotifyingCollections { get; }
        public IGroupTrackerFactory GroupTrackerFactory { get; }
        
        public ObservableGroup(ObservableGroupToken token, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingEntityCollection> notifyingCollections, IGroupTrackerFactory trackerFactory)
        {
            Token = token;
            NotifyingCollections = notifyingCollections;
            GroupTrackerFactory = trackerFactory;

            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new List<IDisposable>();
            GroupTrackers = new Dictionary<int, IObservableGroupTracker>();
            var applicableEntities = initialEntities.Where(x => Token.LookupGroup.Matches(x));
            CachedEntities = new EntityLookup();

            foreach (var applicableEntity in applicableEntities)
            {
                CachedEntities.Add(applicableEntity);
                var trackingSub = GroupTrackerFactory.TrackGroup(applicableEntity, token.LookupGroup);
                trackingSub.OnGroupMatchingChanged.Subscribe(x => OnEntityGroupChanged(applicableEntity, x));
                GroupTrackers.Add(applicableEntity.Id, trackingSub);
            }

            NotifyingCollections.ForEachRun(MonitorEntityChanges);
        }

        private void MonitorEntityChanges(INotifyingEntityCollection notifyingCollection)
        {
            notifyingCollection.EntityAdded
                .Subscribe(OnEntityAddedToCollection)
                .AddTo(Subscriptions);

            notifyingCollection.EntityRemoved
                .Subscribe(OnEntityRemovedFromCollection)
                .AddTo(Subscriptions);
        }

        public void OnEntityGroupChanged(IEntity entity, GroupActionType groupActionType)
        {
            if (groupActionType == GroupActionType.JoinedGroup)
            {
                CachedEntities.Add(entity);
                _onEntityAdded.OnNext(entity);
                return;
            }

            if (groupActionType == GroupActionType.LeavingGroup)
            { _onEntityRemoving.OnNext(entity); }

            if (groupActionType == GroupActionType.LeftGroup)
            {
                CachedEntities.Remove(entity.Id);
                _onEntityRemoved.OnNext(entity);
            }
        }

        public void OnEntityAddedToCollection(CollectionEntityEvent args)
        {
            if (CachedEntities.Contains(args.Entity.Id)) { return; }

            var tracker = GroupTrackerFactory.TrackGroup(args.Entity, Token.LookupGroup);
            tracker.OnGroupMatchingChanged.Subscribe(x => OnEntityGroupChanged(args.Entity, x));
            GroupTrackers.Add(args.Entity.Id, tracker);
            
            if (Token.LookupGroup.Matches(args.Entity))
            {
                CachedEntities.Add(args.Entity);
                _onEntityAdded.OnNext(args.Entity);
            }
        }
        
        public void OnEntityRemovedFromCollection(CollectionEntityEvent args)
        {
            if (!CachedEntities.Contains(args.Entity.Id)) { return; }

            IObservableGroupTracker tracker;
            if (GroupTrackers.TryGetValue(args.Entity.Id, out tracker))
            {
                tracker.Dispose();
                GroupTrackers.Remove(args.Entity.Id);
            }

            CachedEntities.Remove(args.Entity.Id); 
            _onEntityRemoved.OnNext(args.Entity);
        }
        
        public bool ContainsEntity(int id)
        { return CachedEntities.Contains(id); }
        
        public IEntity GetEntity(int id)
        { return CachedEntities[id]; }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
            _onEntityRemoving.Dispose();
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return CachedEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public int Count => CachedEntities.Count;

        public IEntity this[int index] => CachedEntities.GetByIndex(index);
    }
}