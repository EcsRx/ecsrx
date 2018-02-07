using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;

namespace EcsRx.Groups.Accessors
{
    public class CacheableObservableGroup : IObservableGroup, IDisposable
    {
        public readonly IDictionary<Guid, IEntity> CachedEntities;
        public readonly IList<IDisposable> Subscriptions;

        public ObservableGroupToken Token { get; }
        public IEnumerable<IEntity> Entities => CachedEntities.Values;
        public IEventSystem EventSystem { get; }

        public CacheableObservableGroup(ObservableGroupToken token, IEnumerable<IEntity> initialEntities, IEventSystem eventSystem)
        {
            Token = token;
            EventSystem = eventSystem;

            CachedEntities = initialEntities.ToDictionary(x => x.Id, x => x);
            Subscriptions = new List<IDisposable>();
        }

        public void MonitorEntityChanges()
        {
            var addEntitySubscription = EventSystem.Receive<EntityAddedEvent>()
                .Subscribe(OnEntityAddedToPool);

            var removeEntitySubscription = EventSystem.Receive<EntityRemovedEvent>()
                .Where(x => CachedEntities.ContainsKey(x.Entity.Id))
                .Subscribe(OnEntityRemovedFromPool);

            var addComponentSubscription = EventSystem.Receive<ComponentAddedEvent>()
                .Subscribe(OnEntityComponentAdded);

            var removeComponentEntitySubscription = EventSystem.Receive<ComponentRemovedEvent>()
                .Where(x => CachedEntities.ContainsKey(x.Entity.Id))
                .Subscribe(OnEntityComponentRemoved);

            Subscriptions.Add(addEntitySubscription);
            Subscriptions.Add(removeEntitySubscription);
            Subscriptions.Add(addComponentSubscription);
            Subscriptions.Add(removeComponentEntitySubscription);
        }

        public void OnEntityComponentRemoved(ComponentRemovedEvent args)
        {
            if(!args.Entity.HasComponents(Token.ComponentTypes))
            { CachedEntities.Remove(args.Entity.Id); }
        }

        public void OnEntityComponentAdded(ComponentAddedEvent args)
        {
            if(CachedEntities.ContainsKey(args.Entity.Id)) { return; }

            if(args.Entity.HasComponents(Token.ComponentTypes))
            { CachedEntities.Add(args.Entity.Id, args.Entity); }
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
        }

        public void OnEntityRemovedFromPool(EntityRemovedEvent args)
        {
            if (CachedEntities.ContainsKey(args.Entity.Id))
            { CachedEntities.Remove(args.Entity.Id); }
        }

        public void Dispose()
        { Subscriptions.DisposeAll(); }
    }
}