using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Polyfills;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroup : IObservableGroup, IDisposable
    {
        public readonly IDictionary<int, IEntity> CachedEntities;
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

            CachedEntities = initialEntities.Where(x => Token.Group.Matches(x)).ToDictionary(x => x.Id, x => x);
            Subscriptions = new List<IDisposable>();

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
                if (args.Entity.HasAllComponents(Token.Group.RequiredComponents)) 
                { return; }

                _onEntityRemoving.OnNext(args.Entity);
                CachedEntities.Remove(args.Entity.Id);
                _onEntityRemoved.OnNext(args.Entity);
                return;
            }

            if (!Token.Group.Matches(args.Entity)) {return;}
            
            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityComponentRemoving(ComponentsChangedEvent args)
        {
            if (!CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            
            if(Token.Group.ContainsAnyRequiredComponents(args.ComponentTypes))
            { _onEntityRemoving.OnNext(args.Entity); }
        }

        public void OnEntityComponentAdded(ComponentsChangedEvent args)
        {
            if (CachedEntities.ContainsKey(args.Entity.Id))
            {
                if(!Token.Group.ContainsAnyExcludedComponents(args.Entity))
                { return; }

                _onEntityRemoving.OnNext(args.Entity);
                CachedEntities.Remove(args.Entity.Id); 
                _onEntityRemoved.OnNext(args.Entity);
                return;
            }
            
            if (!Token.Group.Matches(args.Entity)) { return; }

            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityAddedToCollection(CollectionEntityEvent args)
        {
            // This is because you may have fired a blueprint before it is created
            if (CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            if (!Token.Group.Matches(args.Entity)) { return; }
            
            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }
        
        public void OnEntityRemovedFromCollection(CollectionEntityEvent args)
        {
            if (!CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            
            _onEntityRemoving.OnNext(args.Entity);
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
        { return CachedEntities.Values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}