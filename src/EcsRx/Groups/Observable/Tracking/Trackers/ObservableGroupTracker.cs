using System;
using EcsRx.Events.Collections;
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
        
        public abstract void UpdateState(int entityId, GroupMatchingType newMatchingType);
        public abstract GroupMatchingType GetState(int entityId);
        public abstract void Dispose();

        public void OnEntityComponentAdded(ComponentsChangedEvent args)
        {
            var entityMatchType = GetState(args.Entity.Id);
            if (entityMatchType == GroupMatchingType.NoMatchesWithExcludes || 
                entityMatchType == GroupMatchingType.MatchesWithExcludes)
            { return; }
            
            if(entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if (LookupGroup.ContainsAnyExcludedComponents(args.ComponentTypeIds))
                {
                    UpdateState(args.Entity.Id, GroupMatchingType.MatchesWithExcludes);
                    OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.LeavingGroup));
                    OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.LeftGroup));
                }
                return;
            }

            if (LookupGroup.ContainsAllRequiredComponents(args.Entity))
            {
                if (LookupGroup.ContainsAnyExcludedComponents(args.Entity))
                {
                    UpdateState(args.Entity.Id, GroupMatchingType.MatchesWithExcludes);
                    return;
                }

                UpdateState(args.Entity.Id, GroupMatchingType.MatchesNoExcludes);
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.JoinedGroup));
            }
        }
        
        public void OnEntityComponentRemoving(ComponentsChangedEvent args)
        {
            var entityMatchType = GetState(args.Entity.Id);
            if (entityMatchType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            if (entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if(LookupGroup.ContainsAnyRequiredComponents(args.ComponentTypeIds))
                { OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.LeavingGroup)); }
            }
        }
        
        public void OnEntityComponentRemoved(ComponentsChangedEvent args)
        {
            var entityMatchType = GetState(args.Entity.Id);
            if (entityMatchType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = LookupGroup.ContainsAllRequiredComponents(args.Entity);
            if (entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if(containsAllComponents)
                { return; }

                UpdateState(args.Entity.Id, GroupMatchingType.NoMatchesNoExcludes);
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.LeftGroup));
            }

            var containsAnyExcluded = LookupGroup.ContainsAnyExcludedComponents(args.Entity);
            
            if (entityMatchType == GroupMatchingType.NoMatchesWithExcludes && !containsAnyExcluded)
            {
                UpdateState(args.Entity.Id, GroupMatchingType.NoMatchesNoExcludes);
                return;
            }
            
            if (entityMatchType == GroupMatchingType.MatchesWithExcludes && containsAllComponents && !containsAnyExcluded)
            {
                UpdateState(args.Entity.Id, GroupMatchingType.MatchesNoExcludes);
                OnGroupMatchingChanged.OnNext(new EntityGroupStateChanged(args.Entity, GroupActionType.JoinedGroup));
                return;
            }
        }
    }
}