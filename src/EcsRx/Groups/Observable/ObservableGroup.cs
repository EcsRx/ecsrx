using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Polyfills;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroup : IObservableGroup, IDisposable
    {
        public readonly Dictionary<Guid, IEntity> CachedEntities;
        public readonly IList<IDisposable> Subscriptions;

        public IObservable<IEntity> OnEntityAdded => _onEntityAdded;
        public IObservable<IEntity> OnEntityRemoved => _onEntityRemoved;

        private readonly Subject<IEntity> _onEntityAdded;
        private readonly Subject<IEntity> _onEntityRemoved;
        
        public ObservableGroupToken Token { get; }
        public IReadOnlyCollection<IEntity> Entities => CachedEntities.Values;
        public IEventSystem EventSystem { get; }

        public ObservableGroup(IEventSystem eventSystem, ObservableGroupToken token, IEnumerable<IEntity> initialEntities)
        {
            Token = token;
            EventSystem = eventSystem;

            _onEntityAdded = new Subject<IEntity>();
            _onEntityRemoved = new Subject<IEntity>();

            CachedEntities = initialEntities.ToDictionary(x => x.Id, x => x);
            Subscriptions = new List<IDisposable>();

            MonitorEntityChanges();
        }

        private void MonitorEntityChanges()
        {
            EventSystem.Receive<EntityAddedEvent>()
                .Subscribe(OnEntityAddedToPool)
                .AddTo(Subscriptions);

            EventSystem.Receive<EntityRemovedEvent>()
                .Subscribe(x =>
                {
                    if(CachedEntities.ContainsKey(x.Entity.Id))
                    { OnEntityRemovedFromPool(x); }
                })
                .AddTo(Subscriptions);

            EventSystem.Receive<ComponentsAddedEvent>()
                .Subscribe(OnEntityComponentAdded)
                .AddTo(Subscriptions);

            EventSystem.Receive<ComponentsRemovedEvent>()
                .Subscribe(x =>
                {
                    if (CachedEntities.ContainsKey(x.Entity.Id))
                    { OnEntityComponentRemoved(x); } 
                })
                .AddTo(Subscriptions);
        }

        public void OnEntityComponentRemoved(ComponentsRemovedEvent args)
        {
            if (args.Entity.HasComponents(Token.ComponentTypes)) { return; }
            
            CachedEntities.Remove(args.Entity.Id);
            _onEntityRemoved.OnNext(args.Entity);
        }

        public void OnEntityComponentAdded(ComponentsAddedEvent args)
        {
            if(CachedEntities.ContainsKey(args.Entity.Id)) { return; }

            if (args.Entity.HasComponents(Token.ComponentTypes))
            {
                CachedEntities.Add(args.Entity.Id, args.Entity);
                _onEntityAdded.OnNext(args.Entity);
            }
        }

        public void OnEntityAddedToPool(EntityAddedEvent args)
        {
            if (!string.IsNullOrEmpty(Token.Pool))
            {
                if(args.EntityCollection.Name != Token.Pool)
                { return; }
            }
            
            if (!args.Entity.Components.Any()) { return; }
            if (!args.Entity.HasComponents(Token.ComponentTypes)) { return; }
            
            CachedEntities.Add(args.Entity.Id, args.Entity);
            _onEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityRemovedFromPool(EntityRemovedEvent args)
        {
            if (!CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            
            CachedEntities.Remove(args.Entity.Id); 
            _onEntityRemoved.OnNext(args.Entity);
        }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            _onEntityAdded.Dispose();
            _onEntityRemoved.Dispose();
        }
    }
}