using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking
{
    public class ObservableGroupTracker : IObservableGroupTracker
    {
        private CompositeDisposable _subs;
        
        public GroupMatchingType CurrentMatchingType { get; private set; }
        public LookupGroup LookupGroup { get; }
        public Subject<GroupActionType> Subject { get; }

        public IObservable<GroupActionType> OnGroupMatchingChanged => Subject;

        public ObservableGroupTracker(IEntity entity, LookupGroup lookupGroup)
        {
            LookupGroup = lookupGroup;
            Subject = new Subject<GroupActionType>();
            _subs = new CompositeDisposable();
            
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity, lookupGroup)).AddTo(_subs);
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity, lookupGroup)).AddTo(_subs);
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity, lookupGroup)).AddTo(_subs);
            
            CurrentMatchingType = CalculateMatchingType(entity, lookupGroup);
        }
        
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
                    Subject.OnNext(GroupActionType.LeavingGroup);
                    Subject.OnNext(GroupActionType.LeftGroup);
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
                Subject.OnNext(GroupActionType.JoinedGroup);
            }
        }
        
        public void OnEntityComponentRemoving(int[] componentsRemoving, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            if (CurrentMatchingType == GroupMatchingType.NoMatchesNoExcludes)
            {
                if(group.ContainsAnyRequiredComponents(componentsRemoving))
                { Subject.OnNext(GroupActionType.LeavingGroup); }
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
                Subject.OnNext(GroupActionType.LeftGroup);
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
                Subject.OnNext(GroupActionType.JoinedGroup);
                return;
            }
        }

        public void Dispose()
        {
            Subject?.Dispose();
            _subs.Dispose();
        }
    }
}