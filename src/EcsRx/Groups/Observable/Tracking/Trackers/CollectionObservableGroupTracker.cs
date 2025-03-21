using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections.Entity;
using EcsRx.Collections.Events;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;

namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public class CollectionObservableGroupTracker : ObservableGroupTracker, ICollectionObservableGroupTracker
    {
        private readonly CompositeDisposable _notifyingSubs;
        public Dictionary<int, GroupMatchingType> EntityIdMatchTypes { get; }
        private IEnumerable<INotifyingCollection> NotifyingEntityComponentChanges { get; }
        private readonly object _lock = new object();

        public CollectionObservableGroupTracker(LookupGroup lookupGroup, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingCollection> notifyingEntityComponentChanges)
            :base(lookupGroup)
        {
            NotifyingEntityComponentChanges = notifyingEntityComponentChanges;
            _notifyingSubs = new CompositeDisposable();

            EntityIdMatchTypes = initialEntities.ToDictionary(x => x.Id, x => LookupGroup.CalculateMatchingType(x));
            NotifyingEntityComponentChanges.ForEachRun(MonitorEntityChanges);
        }
        
        public void MonitorEntityChanges(INotifyingCollection notifier)
        {
            lock (_lock)
            {
                notifier.EntityAdded.Subscribe(OnEntityAdded).AddTo(_notifyingSubs);
                notifier.EntityRemoved.Subscribe(OnEntityRemoved).AddTo(_notifyingSubs);
                
                notifier.EntityComponentsAdded
                    .Subscribe(x => OnEntityComponentAdded(x.ComponentTypeIds, x.Entity))
                    .AddTo(_notifyingSubs);
                
                notifier.EntityComponentsRemoving
                    .Subscribe(x => OnEntityComponentRemoving(x.ComponentTypeIds, x.Entity))
                    .AddTo(_notifyingSubs);
                
                notifier.EntityComponentsRemoved
                    .Subscribe(x => OnEntityComponentRemoved(x.ComponentTypeIds, x.Entity))
                    .AddTo(_notifyingSubs);
            }
        }

        public override void UpdateState(int entityId, GroupMatchingType newMatchingType)
        {
            lock (_lock)
            { EntityIdMatchTypes[entityId] = newMatchingType; }
            
        }

        public override GroupMatchingType GetState(int entityId)
        {
            lock (_lock)
            { return EntityIdMatchTypes.ContainsKey(entityId) ? EntityIdMatchTypes[entityId] : GroupMatchingType.NoMatchesFound; }
        }
        
        public bool IsMatching(int entityId)
        {
            lock (_lock)
            { return EntityIdMatchTypes[entityId] == GroupMatchingType.MatchesNoExcludes; }
        }

        public void OnEntityAdded(CollectionEntityEvent args)
        {
            GroupMatchingType matchType;
            
            lock (_lock)
            {
                if (EntityIdMatchTypes.ContainsKey(args.Entity.Id))
                { return; }
            
                matchType = LookupGroup.CalculateMatchingType(args.Entity);
                EntityIdMatchTypes.Add(args.Entity.Id, matchType);
            }
            
            if(matchType == GroupMatchingType.MatchesNoExcludes)
            { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.JoinedGroup)); }
        }

        public void OnEntityRemoved(CollectionEntityEvent args)
        {
            GroupMatchingType matchType;
            lock (_lock)
            {
                if (!EntityIdMatchTypes.ContainsKey(args.Entity.Id)) { return; }
            
                matchType = GetState(args.Entity.Id);
                EntityIdMatchTypes.Remove(args.Entity.Id);
            }
            
            if (matchType == GroupMatchingType.MatchesNoExcludes)
            {
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.LeavingGroup));
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.LeftGroup));
            }
        }

        public override void Dispose()
        {
            lock (_lock)
            {
                OnGroupMatchingChanged?.Dispose();
                _notifyingSubs.Dispose();
            }
        }
    }
}