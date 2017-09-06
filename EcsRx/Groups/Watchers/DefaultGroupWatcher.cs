using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;

namespace EcsRx.Groups.Watchers
{
    public class DefaultGroupWatcher : IGroupWatcher, IDisposable
    {
        private readonly IList<IDisposable> _subscriptions;

        public IEventSystem EventSystem { get; private set; }
        public Type[] ComponentTypes { get; private set; }

        public Subject<IEntity> OnEntityAdded { get; private set; }
        public Subject<IEntity> OnEntityRemoved { get; private set; }

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
            EventSystem.Receive<ComponentAddedEvent>().Subscribe(OnEntityComponentAdded).AddTo(_subscriptions);
            EventSystem.Receive<ComponentRemovedEvent>().Subscribe(OnEntityComponentRemoved).AddTo(_subscriptions);
        }

        public void Dispose()
        {
            if (OnEntityAdded != null) { OnEntityAdded.Dispose(); }
            if (OnEntityRemoved != null) { OnEntityRemoved.Dispose(); }
            _subscriptions.DisposeAll();
        }

        public void OnEntityComponentRemoved(ComponentRemovedEvent args)
        {
            var originalComponents = args.Entity.Components.Select(x => x.GetType()).ToList();
            originalComponents.Add(args.Component.GetType());

            var matchesGroup = originalComponents.All(x => ComponentTypes.Contains(x));

            if (matchesGroup)
            { OnEntityRemoved.OnNext(args.Entity); }
        }

        public void OnEntityComponentAdded(ComponentAddedEvent args)
        {
            var originalComponentTypes = args.Entity.Components.Select(x => x.GetType()).ToList();
            originalComponentTypes.Remove(args.Component.GetType());

            var previouslyMatched = ComponentTypes.All(x => originalComponentTypes.Contains(x));
            if(previouslyMatched) { return; }

            var newComponentMatches = ComponentTypes.Contains(args.Component.GetType());

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
    }
}