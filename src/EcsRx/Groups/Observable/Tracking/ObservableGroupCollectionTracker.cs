using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking
{
    public class ObservableGroupCollectionTracker : IObservableGroupCollectionTracker
    {
        private CompositeDisposable _notifyingSubs;
        
        public Dictionary<int, GroupMatchingType> EntityIdMatchTypes { get; }
        public LookupGroup LookupGroup { get; }
        public Subject<GroupStateChanged> OnGroupMatchingChanged { get; }

        private IEnumerable<INotifyingEntityComponentChanges> NotifyingEntityComponentChanges { get; }
        public IObservable<GroupStateChanged> GroupMatchingChanged => OnGroupMatchingChanged;

        public ObservableGroupCollectionTracker(LookupGroup lookupGroup, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingEntityComponentChanges> notifyingEntityComponentChanges)
        {
            NotifyingEntityComponentChanges = notifyingEntityComponentChanges;
            LookupGroup = lookupGroup;
            OnGroupMatchingChanged = new Subject<GroupStateChanged>();
            _notifyingSubs = new CompositeDisposable();

            EntityIdMatchTypes = initialEntities.ToDictionary(x => x.Id, x => LookupGroup.CalculateMatchingType(x));
            NotifyingEntityComponentChanges.ForEachRun(MonitorEntityChanges);
        }
        
        public void MonitorEntityChanges(INotifyingEntityComponentChanges notifier)
        {
            notifier.EntityComponentsAdded.Subscribe(OnEntityComponentAdded).AddTo(_notifyingSubs);
            notifier.EntityComponentsRemoving.Subscribe(OnEntityComponentRemoving).AddTo(_notifyingSubs);
            notifier.EntityComponentsRemoved.Subscribe(OnEntityComponentRemoved).AddTo(_notifyingSubs);
        }
        
        public bool IsMatching(int entityId) => EntityIdMatchTypes[entityId] == GroupMatchingType.MatchesNoExcludes;

        public void OnEntityComponentAdded(ComponentsChangedEvent args)
        {
            var entityMatchType = EntityIdMatchTypes[args.Entity.Id];
            if (entityMatchType == GroupMatchingType.NoMatchesWithExcludes || 
                entityMatchType == GroupMatchingType.MatchesWithExcludes)
            { return; }
            
            if(entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if (LookupGroup.ContainsAnyExcludedComponents(args.ComponentTypeIds))
                {
                    EntityIdMatchTypes[args.Entity.Id] = GroupMatchingType.MatchesWithExcludes;
                    OnGroupMatchingChanged.OnNext(new GroupStateChanged(args.Entity, GroupActionType.LeavingGroup));
                    OnGroupMatchingChanged.OnNext(new GroupStateChanged(args.Entity, GroupActionType.LeftGroup));
                }
                return;
            }

            if (LookupGroup.ContainsAllRequiredComponents(args.Entity))
            {
                if (LookupGroup.ContainsAnyExcludedComponents(args.Entity))
                {
                    EntityIdMatchTypes[args.Entity.Id] = GroupMatchingType.MatchesWithExcludes;
                    return;
                }

                EntityIdMatchTypes[args.Entity.Id] = GroupMatchingType.MatchesNoExcludes;
                OnGroupMatchingChanged.OnNext(new GroupStateChanged(args.Entity, GroupActionType.JoinedGroup));
            }
        }
        
        public void OnEntityComponentRemoving(ComponentsChangedEvent args)
        {
            var entityMatchType = EntityIdMatchTypes[args.Entity.Id];
            if (entityMatchType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            if (entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if(LookupGroup.ContainsAnyRequiredComponents(args.ComponentTypeIds))
                { OnGroupMatchingChanged.OnNext(new GroupStateChanged(args.Entity, GroupActionType.LeavingGroup)); }
            }
        }
        
        public void OnEntityComponentRemoved(ComponentsChangedEvent args)
        {
            var entityMatchType = EntityIdMatchTypes[args.Entity.Id];
            if (entityMatchType == GroupMatchingType.NoMatchesNoExcludes)
            { return; }

            var containsAllComponents = LookupGroup.ContainsAllRequiredComponents(args.Entity);
            if (entityMatchType == GroupMatchingType.MatchesNoExcludes)
            {
                if(containsAllComponents)
                { return; }

                EntityIdMatchTypes[args.Entity.Id] = GroupMatchingType.NoMatchesNoExcludes;
                OnGroupMatchingChanged.OnNext(new GroupStateChanged(args.Entity, GroupActionType.LeftGroup));
            }

            var containsAnyExcluded = LookupGroup.ContainsAnyExcludedComponents(args.Entity);
            
            if (entityMatchType == GroupMatchingType.NoMatchesWithExcludes && !containsAnyExcluded)
            {
                EntityIdMatchTypes[args.Entity.Id] = GroupMatchingType.NoMatchesNoExcludes;
                return;
            }
            
            if (entityMatchType == GroupMatchingType.MatchesWithExcludes && containsAllComponents && !containsAnyExcluded)
            {
                EntityIdMatchTypes[args.Entity.Id] = GroupMatchingType.MatchesNoExcludes;
                OnGroupMatchingChanged.OnNext(new GroupStateChanged(args.Entity, GroupActionType.JoinedGroup));
                return;
            }
        }

        public void Dispose()
        {
            OnGroupMatchingChanged?.Dispose();
            _notifyingSubs.Dispose();
        }
    }
}