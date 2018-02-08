using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;

namespace EcsRx.Groups.Accessors
{
    public class ObservableGroup : IObservableGroup, IDisposable
    {
        public readonly IDictionary<Guid, IEntity> CachedEntities;
        public readonly IList<IDisposable> Subscriptions;
        
        public Subject<IEntity> OnEntityAdded { get; }
        public Subject<IEntity> OnEntityRemoved { get; }

        public ObservableGroupToken Token { get; }
        public IEnumerable<IEntity> Entities => CachedEntities.Values;
        public IEventSystem EventSystem { get; }

        public ObservableGroup(IEventSystem eventSystem, ObservableGroupToken token, IEnumerable<IEntity> initialEntities)
        {
            Token = token;
            EventSystem = eventSystem;
            
            OnEntityAdded = new Subject<IEntity>();
            OnEntityRemoved = new Subject<IEntity>();

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
                .Where(x => CachedEntities.ContainsKey(x.Entity.Id))
                .Subscribe(OnEntityRemovedFromPool)
                .AddTo(Subscriptions);

            EventSystem.Receive<ComponentsAddedEvent>()
                .Subscribe(OnEntityComponentAdded)
                .AddTo(Subscriptions);

            EventSystem.Receive<ComponentsRemovedEvent>()
                .Where(x => CachedEntities.ContainsKey(x.Entity.Id))
                .Subscribe(OnEntityComponentRemoved)
                .AddTo(Subscriptions);
        }

        public void OnEntityComponentRemoved(ComponentsRemovedEvent args)
        {
            if (args.Entity.HasComponents(Token.ComponentTypes)) { return; }
            
            CachedEntities.Remove(args.Entity.Id);
            OnEntityRemoved.OnNext(args.Entity);
        }

        public void OnEntityComponentAdded(ComponentsAddedEvent args)
        {
            if(CachedEntities.ContainsKey(args.Entity.Id)) { return; }

            if (args.Entity.HasComponents(Token.ComponentTypes))
            {
                CachedEntities.Add(args.Entity.Id, args.Entity);
                OnEntityAdded.OnNext(args.Entity);
            }
        }

        public void OnEntityAddedToPool(EntityAddedEvent args)
        {
            if (!string.IsNullOrEmpty(Token.Pool))
            {
                if(args.Pool.Name != Token.Pool)
                { return; }
            }
            
            if (!args.Entity.Components.Any()) { return; }
            if (!args.Entity.HasComponents(Token.ComponentTypes)) { return; }
            
            CachedEntities.Add(args.Entity.Id, args.Entity);
            OnEntityAdded.OnNext(args.Entity);
        }

        public void OnEntityRemovedFromPool(EntityRemovedEvent args)
        {
            if (!CachedEntities.ContainsKey(args.Entity.Id)) { return; }
            
            CachedEntities.Remove(args.Entity.Id); 
            OnEntityRemoved.OnNext(args.Entity);
        }

        public void Dispose()
        {
            Subscriptions.DisposeAll();
            OnEntityAdded.Dispose();
            OnEntityRemoved.Dispose();
        }
    }
}