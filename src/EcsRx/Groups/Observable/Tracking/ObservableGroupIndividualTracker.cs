using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking
{
    public class ObservableGroupIndividualTracker : IObservableGroupIndividualTracker
    {
        private CompositeDisposable _subs;
        
        public GroupMatchingType CurrentMatchingType { get; private set; }
        public LookupGroup LookupGroup { get; }
        public Subject<GroupStateChanged> OnGroupMatchingChanged { get; }

        public IObservable<GroupStateChanged> GroupMatchingChanged => OnGroupMatchingChanged;

        public ObservableGroupIndividualTracker(IEntity entity, LookupGroup lookupGroup)
        {
            LookupGroup = lookupGroup;
            OnGroupMatchingChanged = new Subject<GroupStateChanged>();
            _subs = new CompositeDisposable();
            
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity, lookupGroup)).AddTo(_subs);
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity, lookupGroup)).AddTo(_subs);
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity, lookupGroup)).AddTo(_subs);
            
            CurrentMatchingType = CalculateMatchingType(entity, lookupGroup);
        }

        public bool IsMatching() => CurrentMatchingType == GroupMatchingType.MatchesNoExcludes;
        
        public GroupMatchingType CalculateMatchingType(IEntity entity, LookupGroup group)
        {
            var containsAllRequired = group.ContainsAllRequiredComponents(entity);
            var containsAnyExcluded = group.ContainsAnyExcludedComponents(entity);
            
            if(containsAllRequired && containsAnyExcluded) { return GroupMatchingType.MatchesWithExcludes; }
            if(containsAllRequired) { return GroupMatchingType.MatchesNoExcludes; }
            if(containsAnyExcluded) { return GroupMatchingType.NoMatchesWithExcludes; }
            return GroupMatchingType.NoMatchesNoExcludes;
        }

        public void OnEntityComponentAdded(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingType.NoMatchesWithExcludes || 
                CurrentMatchingType == GroupMatchingType.MatchesWithExcludes)
            { return; }
            
            if(CurrentMatchingType == GroupMatchingType.MatchesNoExcludes)
            {
                if (group.ContainsAnyExcludedComponents(componentsAdded))
                {
                    CurrentMatchingType = GroupMatchingType.MatchesWithExcludes;
                    OnGroupMatchingChanged.OnNext(new GroupStateChanged(entity, GroupActionType.LeavingGroup));
                    OnGroupMatchingChanged.OnNext(new GroupStateChanged(entity, GroupActionType.LeftGroup));
                }
                return;
            }

            if (group.ContainsAllRequiredComponents(entity))
            {
                if (group.ContainsAnyExcludedComponents(entity))
                {
                    CurrentMatchingType = GroupMatchingType.MatchesWithExcludes;
                    return;
                }

                CurrentMatchingType = GroupMatchingType.MatchesNoExcludes;
                OnGroupMatchingChanged.OnNext(new GroupStateChanged(entity, GroupActionType.JoinedGroup));
            }
        }
        
        public void OnEntityComponentRemoving(int[] componentsRemoving, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            if (CurrentMatchingType == GroupMatchingType.NoMatchesNoExcludes)
            {
                if(group.ContainsAnyRequiredComponents(componentsRemoving))
                { OnGroupMatchingChanged.OnNext(new GroupStateChanged(entity, GroupActionType.LeavingGroup)); }
            }
        }
        
        public void OnEntityComponentRemoved(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = group.ContainsAllRequiredComponents(entity);
            if (CurrentMatchingType == GroupMatchingType.MatchesNoExcludes)
            {
                if(containsAllComponents)
                { return; }

                CurrentMatchingType = GroupMatchingType.NoMatchesNoExcludes;
                OnGroupMatchingChanged.OnNext(new GroupStateChanged(entity, GroupActionType.LeftGroup));
            }

            var containsAnyExcluded = group.ContainsAnyExcludedComponents(entity);
            
            if (CurrentMatchingType == GroupMatchingType.NoMatchesWithExcludes && !containsAnyExcluded)
            {
                CurrentMatchingType = GroupMatchingType.NoMatchesNoExcludes;
                return;
            }
            
            if (CurrentMatchingType == GroupMatchingType.MatchesWithExcludes && containsAllComponents && !containsAnyExcluded)
            {
                CurrentMatchingType = GroupMatchingType.MatchesNoExcludes;
                OnGroupMatchingChanged.OnNext(new GroupStateChanged(entity, GroupActionType.JoinedGroup));
                return;
            }
        }

        public void Dispose()
        {
            OnGroupMatchingChanged?.Dispose();
            _subs.Dispose();
        }
    }
}