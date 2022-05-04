using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public abstract class ObservableGroupTracker : IObservableGroupTracker
    {
        public LookupGroup LookupGroup { get; }
                
        public Subject<EntityGroupStateChanged> OnGroupMatchingChanged { get; }
        public IObservable<EntityGroupStateChanged> GroupMatchingChanged => OnGroupMatchingChanged;

        protected ObservableGroupTracker(LookupGroup lookupGroup)
        {
            LookupGroup = lookupGroup;
            OnGroupMatchingChanged = new Subject<EntityGroupStateChanged>();
        }

        public abstract void UpdateState(int entityId, GroupMatchingType newMatchingType);
        public abstract GroupMatchingType GetState(int entityId);
        public abstract void Dispose();

        public void OnEntityComponentAdded(int[] componentsChanged, IEntity entity)
        {
            var entityMatchType = GetState(entity.Id);
            if (entityMatchType == GroupMatchingType.NoMatchesWithExcludes || 
                entityMatchType == GroupMatchingType.MatchesWithExcludes)
            { return; }
            
            if(entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if (LookupGroup.ContainsAnyExcludedComponents(componentsChanged))
                {
                    UpdateState(entity.Id, GroupMatchingType.MatchesWithExcludes);
                    OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeavingGroup));
                    OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeftGroup));
                }
                return;
            }

            if (LookupGroup.ContainsAllRequiredComponents(entity))
            {
                if (LookupGroup.ContainsAnyExcludedComponents(entity))
                {
                    UpdateState(entity.Id, GroupMatchingType.MatchesWithExcludes);
                    return;
                }

                UpdateState(entity.Id, GroupMatchingType.MatchesNoExcludes);
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.JoinedGroup));
            }
        }
        
        public void OnEntityComponentRemoving(int[] componentsChanged, IEntity entity)
        {
            var entityMatchType = GetState(entity.Id);
            if (entityMatchType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            if (entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if(LookupGroup.ContainsAnyRequiredComponents(componentsChanged))
                { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeavingGroup)); }
            }
        }
        
        public void OnEntityComponentRemoved(int[] componentsChanged, IEntity entity)
        {
            var entityMatchType = GetState(entity.Id);
            if (entityMatchType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = LookupGroup.ContainsAllRequiredComponents(entity);
            if (entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if(containsAllComponents)
                { return; }

                UpdateState(entity.Id, GroupMatchingType.NoMatchesNoExcludes);
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.LeftGroup));
            }

            var containsAnyExcluded = LookupGroup.ContainsAnyExcludedComponents(entity);
            
            if (entityMatchType == GroupMatchingType.NoMatchesWithExcludes && !containsAnyExcluded)
            {
                UpdateState(entity.Id, GroupMatchingType.NoMatchesNoExcludes);
                return;
            }
            
            if (entityMatchType == GroupMatchingType.MatchesWithExcludes && containsAllComponents && !containsAnyExcluded)
            {
                UpdateState(entity.Id, GroupMatchingType.MatchesNoExcludes);
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(entity, GroupActionType.JoinedGroup));
                return;
            }
        }
    }
}