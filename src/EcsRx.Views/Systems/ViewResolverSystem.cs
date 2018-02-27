using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.ViewHandlers;

namespace EcsRx.Views.Systems
{
    public abstract class ViewResolverSystem : ISetupSystem, IDisposable
    {
        private readonly IDisposable _destructionSubscription;
        private readonly IDictionary<Guid, object> _viewCache = new Dictionary<Guid, object>();
        
        public IEventSystem EventSystem { get; }

        public abstract IViewHandler ViewHandler { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));

        protected ViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;

            _destructionSubscription = EventSystem.Receive<ComponentsRemovedEvent>()
                .Where(x => _viewCache.ContainsKey(x.Entity.Id))
                .Where(x => x.Components.Any(y => y is ViewComponent))
                .Subscribe(x => OnViewRemoved(x.Entity));
        }

        protected virtual void OnViewRemoved(IEntity entity)
        {
            var view = _viewCache[entity.Id];
            _viewCache.Remove(entity.Id);
            ViewHandler.DestroyView(view);
        }

        protected abstract void OnViewCreated(IEntity entity, ViewComponent view);

        public void Dispose()
        { _destructionSubscription.Dispose(); }

        public virtual void Setup(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            if (viewComponent.View != null) { return; }

            viewComponent.View = ViewHandler.CreateView();
            _viewCache.Add(entity.Id, viewComponent.View);

            OnViewCreated(entity, viewComponent);
        }
    }
}