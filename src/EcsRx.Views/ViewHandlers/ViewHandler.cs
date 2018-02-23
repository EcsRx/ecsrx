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
    public abstract class ViewHandler : IViewHandler, IDisposable
    {
        public IPoolManager PoolManager { get; }
        public IEventSystem EventSystem { get; }
        public Func<IEntity, object> ViewResolver { get; }

        private readonly IDisposable _destructionSubscription;
        private readonly IDictionary<Guid, object> _viewCache = new Dictionary<Guid, object>();

        protected ViewHandler(IPoolManager poolManager, IEventSystem eventSystem, Func<IEntity, object> viewResolver)
        {
            PoolManager = poolManager;
            EventSystem = eventSystem;
            ViewResolver = viewResolver;

            _destructionSubscription = EventSystem.Receive<ComponentsRemovedEvent>()
                .Where(x => _viewCache.ContainsKey(x.Entity.Id))
                .Where(x => x.Components.Any(y => y is ViewComponent))
                .Subscribe(OnComponentRemoved);
        }

        private void OnComponentRemoved(ComponentsRemovedEvent x)
        {
            var view = _viewCache[x.Entity.Id];
            _viewCache.Remove(x.Entity.Id);
            DestroyView(view);
        }

        public abstract void DestroyView(object view);
        public abstract void OnViewCreated(IEntity entity, object view);
        
        public void SetupView(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            if (viewComponent.View != null) { return; }

            var viewObject = ViewResolver(entity);
            viewComponent.View = viewObject;          
            
            OnViewCreated(entity, ViewResolver);
        }

        public void Dispose()
        { _destructionSubscription.Dispose(); }
    }
}