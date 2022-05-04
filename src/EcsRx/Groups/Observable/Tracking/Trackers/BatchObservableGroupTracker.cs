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
        public Dictionary<int, GroupMatchingType> EntityIdMatchTypes { get; }

        public BatchObservableGroupTracker(LookupGroup lookupGroup) : base(lookupGroup)
        {
            _entitySubscriptions = new Dictionary<int, IDisposable>();
            EntityIdMatchTypes = new Dictionary<int, GroupMatchingType>();
        }
        
        public bool IsMatching(int entityId) => EntityIdMatchTypes[entityId] == GroupMatchingType.MatchesNoExcludes;
        
        public override void UpdateState(int entityId, GroupMatchingType newMatchingType)
        { EntityIdMatchTypes[entityId] = newMatchingType; }

        public override GroupMatchingType GetState(int entityId)
        { return EntityIdMatchTypes[entityId]; }
        
        public bool StartTrackingEntity(IEntity entity)
        {
            if (EntityIdMatchTypes.ContainsKey(entity.Id))
            { return EntityIdMatchTypes[entity.Id] == GroupMatchingType.MatchesNoExcludes; }
            
            var matchingType = LookupGroup.CalculateMatchingType(entity);
            EntityIdMatchTypes.Add(entity.Id, matchingType);
            
            var entitySubs = new CompositeDisposable();
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity)).AddTo(entitySubs);
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity)).AddTo(entitySubs);
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity)).AddTo(entitySubs);
            _entitySubscriptions.Add(entity.Id, entitySubs);
            
            return matchingType == GroupMatchingType.MatchesNoExcludes;
        }

        public void StopTrackingEntity(IEntity entity)
        {
            if (!EntityIdMatchTypes.ContainsKey(entity.Id)) {return;}

            if (EntityIdMatchTypes[entity.Id] == GroupMatchingType.MatchesNoExcludes)
            {
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeavingGroup));
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeftGroup));
            }
            
            _entitySubscriptions[entity.Id].Dispose();
            _entitySubscriptions.Remove(entity.Id);
            EntityIdMatchTypes.Remove(entity.Id);
        }
        
        public override void Dispose()
        {
            OnGroupMatchingChanged?.Dispose();
            _entitySubscriptions.DisposeAll();
        }
    }
}