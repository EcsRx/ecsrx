using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Lookups;
using EcsRx.MicroRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroup : IObservableGroup, IDisposable
    {
        public readonly EntityLookup CachedEntities;
        public readonly List<IDisposable> Subscriptions;

        public IObservable<IEntity> OnEntityAdded => _onEntityAdded;
        public IObservable<IEntity> OnEntityRemoved => _onEntityRemoved;
        public IObservable<IEntity> OnEntityRemoving => _onEntityRemoving;

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        private readonly Subject<IEntity> _onEntityRemoving;
        
        public ObservableGroupToken Token { get; }
        public IEnumerable<INotifyingEntityCollection> NotifyingCollections { get; }
        
        public ObservableGroup(ObservableGroupToken token, IEnumerable<IEntity> initialEntities, IEnumerable<INotifyingEntityCollection> notifyingCollections)
        {
            Token = token;
            NotifyingCollections = notifyingCollections;

            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new List<IDisposable>();
            var applicableEntities = initialEntities.Where(x => Token.LookupGroup.Matches(x));
            CachedEntities = new EntityLookup();

            foreach (var applicableEntity in applicableEntities)
            { CachedEntities.Add(applicableEntity); }

            NotifyingCollections.ForEachRun(MonitorEntityChanges);
        }

        private void MonitorEntityChanges(INotifyingEntityCollection notifyingCollection)
        {
            notifyingCollection.EntityAdded
                .Subscribe(OnEntityAddedToCollection)
                .AddTo(Subscriptions);

            notifyingCollection.EntityRemoved
                .Subscribe(OnEntityRemovedFromCollection)
                .AddTo(Subscriptions);

            notifyingCollection.EntityComponentsAdded
                .Subscribe(OnEntityComponentAdded)
                .AddTo(Subscriptions);

            notifyingCollection.EntityComponentsRemoving
                .Subscribe(OnEntityComponentRemoving)
                .AddTo(Subscriptions);

            notifyingCollection.EntityComponentsRemoved
                .Subscribe(OnEntityComponentRemoved)
                .AddTo(Subscriptions);
        }

        public void OnEntityComponentRemoved(ComponentsChangedEvent args)
        {
            if (CachedEntities.Contains(args.Entity.Id))
            {
                if (args.Entity.HasAllComponents(Token.LookupGroup.RequiredComponents)) 
                { return; }

                CachedEntities.Remove(args.Entity.Id);
                _onEntityRemoved.OnNext(args.Entity);
                return;
            }

            if (!Token.LookupGroup.Matches(args.Entity)) {return;}
            
            CachedEntities.Add(args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityComponentRemoving(ComponentsChangedEvent args)
        {
            if (!CachedEntities.Contains(args.Entity.Id)) { return; }
            
            if(Token.LookupGroup.ContainsAnyRequiredComponents(args.ComponentTypeIds))
            { _onEntityRemoving.OnNext(args.Entity); }
        }

        public void OnEntityComponentAdded(ComponentsChangedEvent args)
        {
            if (CachedEntities.Contains(args.Entity.Id))
            {
                if(!Token.LookupGroup.ContainsAnyExcludedComponents(args.Entity))
                { return; }

                _onEntityRemoving.OnNext(args.Entity);
                CachedEntities.Remove(args.Entity.Id); 
                _onEntityRemoved.OnNext(args.Entity);
                return;
            }
            
            if (!Token.LookupGroup.Matches(args.Entity)) { return; }

            CachedEntities.Add(args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityAddedToCollection(CollectionEntityEvent args)
        {
            // This is because you may have fired a blueprint before it is created
            if (CachedEntities.Contains(args.Entity.Id)) { return; }
            if (!Token.LookupGroup.Matches(args.Entity)) { return; }
            
            CachedEntities.Add(args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }
        
        public void OnEntityRemovedFromCollection(CollectionEntityEvent args)
        {
            if (!CachedEntities.Contains(args.Entity.Id)) { return; }
            
            CachedEntities.Remove(args.Entity.Id); 
            _onEntityRemoved.OnNext(args.Entity);
        }
        
        public bool ContainsEntity(int id)
        { return CachedEntities.Contains(id); }
        
        public IEntity GetEntity(int id)
        { return CachedEntities[id]; }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
            _onEntityRemoving.Dispose();
        }

        public IEnumerator<IEntity> GetEnumerator()
        { return CachedEntities.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public int Count => CachedEntities.Count;

        public IEntity this[int index] => CachedEntities.GetByIndex(index);
    }
}