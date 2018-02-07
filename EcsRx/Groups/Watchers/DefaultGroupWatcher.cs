using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;

namespace EcsRx.Groups.Watchers
{
    public class DefaultGroupWatcher : IGroupWatcher
    {
        private readonly IList<IDisposable> _subscriptions;

        public IEventSystem EventSystem { get; }
        public Type[] ComponentTypes { get; }

        public Subject<IEntity> OnEntityAdded { get; }
        public Subject<IEntity> OnEntityRemoved { get; }

        public DefaultGroupWatcher(IEventSystem eventSystem, Type[] componentTypes)
        {
            EventSystem = eventSystem;
            ComponentTypes = componentTypes;

            OnEntityAdded = new Subject<IEntity>();
            OnEntityRemoved = new Subject<IEntity>();

            _subscriptions = new List<IDisposable>();
            SetupSubscriptions();
        }

        private void SetupSubscriptions()
        {
            EventSystem.Receive<EntityAddedEvent>().Subscribe(OnEntityAddedToPool).AddTo(_subscriptions);
            EventSystem.Receive<EntityRemovedEvent>().Subscribe(OnEntityRemovedFromPool).AddTo(_subscriptions);
            EventSystem.Receive<ComponentsAddedEvent>().Subscribe(OnEntityComponentAdded).AddTo(_subscriptions);
            EventSystem.Receive<ComponentRemovedEvent>().Subscribe(OnEntityComponentRemoved).AddTo(_subscriptions);
        }

        public void OnEntityComponentRemoved(ComponentRemovedEvent args)
        {
            var originalComponents = args.Entity.Components.Select(x => x.GetType()).ToList();
            originalComponents.Add(args.Component.GetType());

            var matchesGroup = originalComponents.All(ComponentTypes.Contains);

            if (matchesGroup)
            { OnEntityRemoved.OnNext(args.Entity); }
        }

        public void OnEntityComponentAdded(ComponentsAddedEvent args)
        {
            var originalComponentTypes = args.Entity.Components.Select(x => x.GetType()).ToList();
            var addedComponents = args.Components.Select(y => y.GetType());
            foreach (var component in addedComponents)
            { originalComponentTypes.Remove(component); }
            
            var previouslyMatched = ComponentTypes.All(originalComponentTypes.Contains);
            if(previouslyMatched) { return; }

            var newComponentMatches = addedComponents.All(ComponentTypes.Contains);
            if (newComponentMatches)
            { OnEntityAdded.OnNext(args.Entity); }
        }

        public void OnEntityAddedToPool(EntityAddedEvent args)
        {
            if (!args.Entity.Components.Any()) { return; }

            var matchesGroup = args.Entity.HasComponents(ComponentTypes);
            if(matchesGroup)
            { OnEntityAdded.OnNext(args.Entity); }
        }

        public void OnEntityRemovedFromPool(EntityRemovedEvent args)
        {
            if (!args.Entity.Components.Any()) { return; }

            var matchesGroup = args.Entity.HasComponents(ComponentTypes);
            if (matchesGroup)
            { OnEntityRemoved.OnNext(args.Entity); }
        }
        
        public void Dispose()
        { _subscriptions.DisposeAll(); }
    }
}