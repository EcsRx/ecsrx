﻿using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections.Database;
using EcsRx.Components.Lookups;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Lookups;

namespace EcsRx.Collections
{
    public class ObservableGroupManager : IObservableGroupManager
    {
        public ObservableGroupLookup _observableGroups { get; }

        public IReadOnlyList<IObservableGroup> ObservableGroups => _observableGroups;

        public IEntityDatabase EntityDatabase { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        private readonly object _lock = new object();
        
        public ObservableGroupManager(IObservableGroupFactory observableGroupFactory, IEntityDatabase entityDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ObservableGroupFactory = observableGroupFactory;
            EntityDatabase = entityDatabase;
            ComponentTypeLookup = componentTypeLookup;

            _observableGroups = new ObservableGroupLookup();
        }

        public IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds)
        {
            lock (_lock)
            {
                for (var i = _observableGroups.Count - 1; i >= 0; i--)
                {
                    if (_observableGroups[i].Token.LookupGroup.Matches(componentTypeIds))
                    { yield return _observableGroups[i]; }
                }
            }
        }

        public IObservableGroup GetObservableGroup(IGroup group, params int[] collectionIds)
        {
            var lookupGroup = ComponentTypeLookup.GetLookupGroupFor(group);
            var observableGroupToken = new ObservableGroupToken(lookupGroup, collectionIds);
            
            lock (_lock)
            {
                if (_observableGroups.Contains(observableGroupToken)) 
                { return _observableGroups[observableGroupToken]; }
            }

            var configuration = new ObservableGroupConfiguration
            {
                ObservableGroupToken = observableGroupToken
            };

            lock (_lock)
            {
                if (collectionIds != null && collectionIds.Length > 0)
                {
                    var targetedCollections = EntityDatabase.Collections.Where(x => collectionIds.Contains(x.Id));
                    configuration.NotifyingCollections = targetedCollections;
                    configuration.InitialEntities = targetedCollections.GetAllEntities();
                }
                else
                {
                    configuration.NotifyingCollections = new[] { EntityDatabase };
                    configuration.InitialEntities = EntityDatabase.Collections.GetAllEntities();
                }
                
                var observableGroup = ObservableGroupFactory.Create(configuration);
                _observableGroups.Add(observableGroup);

                return observableGroup;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var observableGroup in _observableGroups)
                { observableGroup?.Dispose(); }

                EntityDatabase.Dispose();
            }
        }
    }
}