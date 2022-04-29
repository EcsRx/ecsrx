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
        
        public GroupMatchingTypes CurrentMatchingType { get; private set; }
        public LookupGroup LookupGroup { get; }
        public Subject<GroupActionTypes> Subject { get; }

        public IObservable<GroupActionTypes> OnGroupMatchingChanged => Subject;

        public ObservableGroupTracker(IEntity entity, LookupGroup lookupGroup)
        {
            LookupGroup = lookupGroup;
            CurrentMatchingType = CalculateMatchingType(entity, lookupGroup);
            Subject = new Subject<GroupActionTypes>();
            
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity, lookupGroup)).AddTo(_subs);
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity, lookupGroup)).AddTo(_subs);
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity, lookupGroup)).AddTo(_subs);
        }

        public GroupMatchingTypes CalculateMatchingType(IEntity entity, LookupGroup group)
        {
            var containsAllRequired = group.ContainsAllRequiredComponents(entity);
            var containsAnyExcluded = group.ContainsAnyExcludedComponents(entity);
            
            if(containsAllRequired && containsAnyExcluded) { return GroupMatchingTypes.MatchesWithExcludes; }
            if(containsAllRequired) { return GroupMatchingTypes.MatchesNoExcludes; }
            if(containsAnyExcluded) { return GroupMatchingTypes.NoMatchesWithExcludes; }
            return GroupMatchingTypes.NoMatchesNoExcludes;
        }

        public void OnEntityComponentAdded(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingTypes.NoMatchesWithExcludes || 
                CurrentMatchingType == GroupMatchingTypes.MatchesWithExcludes)
            { return; }
            
            if(CurrentMatchingType == GroupMatchingTypes.MatchesNoExcludes)
            {
                if (group.ContainsAnyExcludedComponents(componentsAdded))
                {
                    CurrentMatchingType = GroupMatchingTypes.MatchesWithExcludes;
                    Subject.OnNext(GroupActionTypes.LeavingGroup);
                    Subject.OnNext(GroupActionTypes.LeftGroup);
                }
                return;
            }

            if (group.ContainsAllRequiredComponents(entity))
            {
                if (group.ContainsAnyExcludedComponents(entity))
                {
                    CurrentMatchingType = GroupMatchingTypes.MatchesWithExcludes;
                    return;
                }

                CurrentMatchingType = GroupMatchingTypes.MatchesNoExcludes;
                Subject.OnNext(GroupActionTypes.JoinedGroup);
            }
        }
        
        public void OnEntityComponentRemoving(int[] componentsRemoving, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingTypes.NoMatchesNoExcludes)
            { return; }

            if (CurrentMatchingType == GroupMatchingTypes.NoMatchesNoExcludes)
            {
                if(group.ContainsAnyRequiredComponents(componentsRemoving))
                { Subject.OnNext(GroupActionTypes.LeavingGroup); }
            }
        }
        
        public void OnEntityComponentRemoved(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            if (CurrentMatchingType == GroupMatchingTypes.NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = group.ContainsAllRequiredComponents(entity);
            if (CurrentMatchingType == GroupMatchingTypes.MatchesNoExcludes)
            {
                if(containsAllComponents)
                { return; }

                CurrentMatchingType = GroupMatchingTypes.NoMatchesNoExcludes;
                Subject.OnNext(GroupActionTypes.LeftGroup);
            }

            var containsAnyExcluded = group.ContainsAnyExcludedComponents(entity);
            
            if (CurrentMatchingType == GroupMatchingTypes.NoMatchesWithExcludes && !containsAnyExcluded)
            {
                CurrentMatchingType = GroupMatchingTypes.NoMatchesNoExcludes;
                return;
            }
            
            if (CurrentMatchingType == GroupMatchingTypes.MatchesWithExcludes && containsAllComponents && !containsAnyExcluded)
            {
                CurrentMatchingType = GroupMatchingTypes.MatchesNoExcludes;
                Subject.OnNext(GroupActionTypes.JoinedGroup);
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