using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Events.Collections;
using EcsRx.Extensions;
using EcsRx.Lookups;
using EcsRx.MicroRx;
using EcsRx.MicroRx.Extensions;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroup : IObservableGroup, IDisposable
    {
        public readonly IterableDictionary<int, IEntity> CachedEntities;
        public readonly IList<IDisposable> Subscriptions;

        public IObservable<IEntity> OnEntityAdded => _onEntityAdded;
        public IObservable<IEntity> OnEntityRemoved => _onEntityRemoved;
        public IObservable<IEntity> OnEntityRemoving => _onEntityRemoving;

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        private readonly Subject<IEntity> _onEntityRemoving;
        
        public ObservableGroupToken Token { get; }
        public INotifyingEntityCollection NotifyingCollection { get; }
        
        public ObservableGroup(ObservableGroupToken token, IEnumerable<IEntity> initialEntities, INotifyingEntityCollection notifyingCollection)
        {
            Token = token;
            NotifyingCollection = notifyingCollection;

            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();
            _onEntityRemoving = new Subject<IEntity>();

            Subscriptions = new List<IDisposable>();
            var applicableEntities = initialEntities.Where(x => Token.LookupGroup.Matches(x));
            CachedEntities = new IterableDictionary<int, IEntity>();

            foreach (var applicableEntity in applicableEntities)
            { CachedEntities.Add(applicableEntity.Id, applicableEntity); }

            MonitorEntityChanges();
        }

        private void MonitorEntityChanges()
        {
            NotifyingCollection.EntityAdded
                .Subscribe(OnEntityAddedToCollection)
                .AddTo(Subscriptions);

            NotifyingCollection.EntityRemoved
                .Subscribe(OnEntityRemovedFromCollection)
                .AddTo(Subscriptions);

            NotifyingCollection.EntityComponentsAdded
                .Subscribe(OnEntityComponentAdded)
                .AddTo(Subscriptions);

            NotifyingCollection.EntityComponentsRemoving
                .Subscribe(OnEntityComponentRemoving)
                .AddTo(Subscriptions);

            NotifyingCollection.EntityComponentsRemoved
                .Subscribe(OnEntityComponentRemoved)
                .AddTo(Subscriptions);
        }

        public void OnEntityComponentRemoved(ComponentsChangedEvent args)
        {
            if (CachedEntities.ContainsKey(args.Entity.Id))
            {
                if (args.Entity.HasAllComponents(Token.LookupGroup.RequiredComponents)) 
                { return; }

                CachedEntities.Remove(args.Entity.Id);
                _onEntityRemoved.OnNext(args.Entity);
                return;
            }

            if (!Token.LookupGroup.Matches(args.Entity)) {return;}
            
            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityComponentRemoving(ComponentsChangedEvent args)
        {
            if (!CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            
            if(Token.LookupGroup.ContainsAnyRequiredComponents(args.ComponentTypeIds))
            { _onEntityRemoving.OnNext(args.Entity); }
        }

        public void OnEntityComponentAdded(ComponentsChangedEvent args)
        {
            if (CachedEntities.ContainsKey(args.Entity.Id))
            {
                if(!Token.LookupGroup.ContainsAnyExcludedComponents(args.Entity))
                { return; }

                _onEntityRemoving.OnNext(args.Entity);
                CachedEntities.Remove(args.Entity.Id); 
                _onEntityRemoved.OnNext(args.Entity);
                return;
            }
            
            if (!Token.LookupGroup.Matches(args.Entity)) { return; }

            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityAddedToCollection(CollectionEntityEvent args)
        {
            // This is because you may have fired a blueprint before it is created
            if (CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            if (!Token.LookupGroup.Matches(args.Entity)) { return; }
            
            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }
        
        public void OnEntityRemovedFromCollection(CollectionEntityEvent args)
        {
            if (!CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            
            CachedEntities.Remove(args.Entity.Id); 
            _onEntityRemoved.OnNext(args.Entity);
        }
        
        public bool ContainsEntity(int id)
        { return CachedEntities.ContainsKey(id); }

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

        public IEntity this[int index] => CachedEntities[index];
    }
}