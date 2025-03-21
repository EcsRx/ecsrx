using System;
using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;

namespace EcsRx.Groups.Observable.Tracking.Trackers
{
public class BatchObservableGroupTracker : ObservableGroupTracker, IBatchObservableGroupTracker
    {
        private readonly Dictionary<int, IDisposable> _entitySubscriptions;
        private readonly object _lock = new object();
        
        public Dictionary<int, GroupMatchingType> EntityIdMatchTypes { get; }

        public BatchObservableGroupTracker(LookupGroup lookupGroup) : base(lookupGroup)
        {
            _entitySubscriptions = new Dictionary<int, IDisposable>();
            EntityIdMatchTypes = new Dictionary<int, GroupMatchingType>();
        }
        
        public bool IsMatching(int entityId)
        {
            lock (_lock)
            { return EntityIdMatchTypes[entityId] == GroupMatchingType.MatchesNoExcludes; }
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
        
        public bool StartTrackingEntity(IEntity entity)
        {
            lock (_lock)
            {
                if (EntityIdMatchTypes.ContainsKey(entity.Id))
                { return EntityIdMatchTypes[entity.Id] == GroupMatchingType.MatchesNoExcludes; }
            }
            
            var matchingType = LookupGroup.CalculateMatchingType(entity);
            var entitySubs = new CompositeDisposable();
            
            lock (_lock)
            {
                EntityIdMatchTypes.Add(entity.Id, matchingType);
                _entitySubscriptions.Add(entity.Id, entitySubs);
            }
            
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity)).AddTo(entitySubs);
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity)).AddTo(entitySubs);
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity)).AddTo(entitySubs);
            
            return matchingType == GroupMatchingType.MatchesNoExcludes;
        }

        public void StopTrackingEntity(IEntity entity)
        {
            GroupMatchingType matchType;
            
            lock (_lock)
            {
                if (!EntityIdMatchTypes.ContainsKey(entity.Id)) { return; }

                matchType = EntityIdMatchTypes[entity.Id];
                _entitySubscriptions[entity.Id].Dispose();
                _entitySubscriptions.Remove(entity.Id);
                EntityIdMatchTypes.Remove(entity.Id);
            }

            if (matchType == GroupMatchingType.MatchesNoExcludes)
            {
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeavingGroup));
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeftGroup));
            }
        }
        
        public override void Dispose()
        {
            lock (_lock)
            {
                OnGroupMatchingChanged?.Dispose();
                _entitySubscriptions.DisposeAll();
            }
        }
    }
}