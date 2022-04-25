using System;
using System.Collections.Generic;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Extensions;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking
{
    public class ObservableGroupTracker : IObservableGroupTracker
    {
        public const int MatchesNoExcludes = 1;
        public const int MatchesWithExcludes = 2;
        public const int NoMatchesWithExcludes = 3;
        public const int NoMatchesNoExcludes = 4;
        
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public Dictionary<int, int> EntityStates { get; }
        public Dictionary<int, Subject<GroupMatchingState>> EntitySubjects { get; }

        public ObservableGroupTracker(IComponentTypeLookup componentTypeLookup)
        {
            ComponentTypeLookup = componentTypeLookup;
            EntityStates = new Dictionary<int, int>();
            EntitySubjects = new Dictionary<int, Subject<GroupMatchingState>>();
        }

        public IObservable<GroupMatchingState> OnGroupMatchingChanged(IEntity entity, IGroup group)
        { return OnGroupMatchingChanged(entity, ComponentTypeLookup.GetLookupGroupFor(group)); }

        public IObservable<GroupMatchingState> OnGroupMatchingChanged(IEntity entity, LookupGroup group)
        {
            var startingState = GenerateStartingState(entity, group);
            var matchingSubject = new Subject<GroupMatchingState>();
            EntityStates[entity.Id] = startingState;
            EntitySubjects[entity.Id] = matchingSubject;
            
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity, group));
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity, group));
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity, group));

            return matchingSubject;
        }

        public int GenerateStartingState(IEntity entity, LookupGroup group)
        {
            var containsAllRequired = group.ContainsAllRequiredComponents(entity);
            var containsAnyExcluded = group.ContainsAnyExcludedComponents(entity);
            
            if(containsAllRequired && containsAnyExcluded) { return MatchesWithExcludes; }
            if(containsAllRequired) { return MatchesNoExcludes; }
            if(containsAnyExcluded) { return NoMatchesWithExcludes; }
            return NoMatchesNoExcludes;
        }

        public void OnEntityComponentAdded(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            var entityState = EntityStates[entity.Id];
            if (entityState == NoMatchesWithExcludes || entityState == MatchesWithExcludes)
            { return; }
            
            if(entityState == MatchesNoExcludes)
            {
                if (group.ContainsAnyExcludedComponents(componentsAdded))
                {
                    EntityStates[entity.Id] = MatchesWithExcludes;
                    EntitySubjects[entity.Id].OnNext(GroupMatchingState.LeavingGroup);
                    EntitySubjects[entity.Id].OnNext(GroupMatchingState.LeftGroup);
                }
                return;
            }

            if (group.ContainsAllRequiredComponents(entity))
            {
                if (group.ContainsAnyExcludedComponents(entity))
                {
                    EntityStates[entity.Id] = MatchesWithExcludes;
                    return;
                }

                EntityStates[entity.Id] = MatchesNoExcludes;
                EntitySubjects[entity.Id].OnNext(GroupMatchingState.JoinedGroup);
            }
        }
        
        public void OnEntityComponentRemoving(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            var entityState = EntityStates[entity.Id];
            if (entityState == NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = group.ContainsAllRequiredComponents(entity);
            if (entityState == MatchesNoExcludes && !containsAllComponents)
            { EntitySubjects[entity.Id].OnNext(GroupMatchingState.LeavingGroup); }
        }
        
        public void OnEntityComponentRemoved(int[] componentsAdded, IEntity entity, LookupGroup group)
        {
            var entityState = EntityStates[entity.Id];
            if (entityState == NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = group.ContainsAllRequiredComponents(entity);
            if (entityState == MatchesNoExcludes)
            {
                if(containsAllComponents)
                { return; }

                EntityStates[entity.Id] = NoMatchesNoExcludes;
                EntitySubjects[entity.Id].OnNext(GroupMatchingState.LeftGroup);
            }

            var containsAnyExcluded = group.ContainsAnyExcludedComponents(entity);
            
            if (entityState == NoMatchesWithExcludes && !containsAnyExcluded)
            {
                EntityStates[entity.Id] = NoMatchesNoExcludes;
                return;
            }
            
            if (entityState == MatchesWithExcludes && containsAllComponents && !containsAnyExcluded)
            {
                EntityStates[entity.Id] = MatchesNoExcludes;
                EntitySubjects[entity.Id].OnNext(GroupMatchingState.JoinedGroup);
                return;
            }
        }
    }
}