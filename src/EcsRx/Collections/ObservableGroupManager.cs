using System;
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
    public class ObservableGroupManager : IObservableGroupManager, IDisposable
    {
        private readonly ObservableGroupLookup _observableGroups;

        public IReadOnlyList<IObservableGroup> ObservableGroups => _observableGroups;

        public IEntityDatabase EntityDatabase { get; }
        public IObservableGroupFactory ObservableGroupFactory { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        
        public ObservableGroupManager(IObservableGroupFactory observableGroupFactory, IEntityDatabase entityDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ObservableGroupFactory = observableGroupFactory;
            EntityDatabase = entityDatabase;
            ComponentTypeLookup = componentTypeLookup;

            _observableGroups = new ObservableGroupLookup();
        }

        public IEnumerable<IObservableGroup> GetApplicableGroups(int[] componentTypeIds)
        {
            for (var i = _observableGroups.Count - 1; i >= 0; i--)
            {
                if (_observableGroups[i].Token.LookupGroup.Matches(componentTypeIds))
                { yield return _observableGroups[i]; }
            }
        }

        public IObservableGroup GetObservableGroup(IGroup group, params int[] collectionIds)
        {
            var requiredComponents = ComponentTypeLookup.GetComponentTypes(group.RequiredComponents);
            var excludedComponents = ComponentTypeLookup.GetComponentTypes(group.ExcludedComponents);
            var lookupGroup = new LookupGroup(requiredComponents, excludedComponents);
            
            var observableGroupToken = new ObservableGroupToken(lookupGroup, collectionIds);
            if (_observableGroups.Contains(observableGroupToken)) 
            { return _observableGroups[observableGroupToken]; }

            var entityMatches = EntityDatabase.GetEntitiesFor(lookupGroup, collectionIds);
            var configuration = new ObservableGroupConfiguration
            {
                ObservableGroupToken = observableGroupToken,
                InitialEntities = entityMatches
            };

            if (collectionIds != null && collectionIds.Length > 0)
            { configuration.NotifyingCollections = EntityDatabase.Collections.Where(x => collectionIds.Contains(x.Id)); }
            else
            { configuration.NotifyingCollections = new []{EntityDatabase}; }
            
            var observableGroup = ObservableGroupFactory.Create(configuration);
            _observableGroups.Add(observableGroup);

            return _observableGroups[observableGroupToken];
        }

        public void Dispose()
        {
            foreach (var observableGroup in _observableGroups)
            { (observableGroup as IDisposable)?.Dispose(); }

            EntityDatabase.Dispose();
        }
    }
}