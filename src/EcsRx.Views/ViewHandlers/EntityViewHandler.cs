using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Pools;
using EcsRx.Views.Components;

namespace EcsRx.Views.ViewHandlers
{
    public abstract class EntityViewHandler : IEntityViewHandler, IDisposable
    {
        public IPoolManager PoolManager { get; }
        public IEventSystem EventSystem { get; }

        public abstract IViewHandler ViewHandler { get; }
        
        private readonly IDisposable _destructionSubscription;
        private readonly IDictionary<Guid, object> _viewCache = new Dictionary<Guid, object>();

        protected EntityViewHandler(IPoolManager poolManager, IEventSystem eventSystem)
        {
            PoolManager = poolManager;
            EventSystem = eventSystem;

            _destructionSubscription = EventSystem.Receive<ComponentsRemovedEvent>()
                .Where(x => _viewCache.ContainsKey(x.Entity.Id))
                .Where(x => x.Components.Any(y => y is ViewComponent))
                .Subscribe(OnViewRemoved);
        }

        protected virtual void OnViewRemoved(ComponentsRemovedEvent x)
        {
            var view = _viewCache[x.Entity.Id];
            _viewCache.Remove(x.Entity.Id);
            ViewHandler.DestroyView(view);
        }

        protected abstract void OnViewCreated(IEntity entity, ViewComponent view);
        
        public void SetupView(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            if (viewComponent.View != null) { return; }

            viewComponent.View = ViewHandler.CreateView();          
            
            OnViewCreated(entity, viewComponent);
        }

        public void Dispose()
        { _destructionSubscription.Dispose(); }
    }
}