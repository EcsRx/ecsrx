using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Polyfills;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.Pooling;

namespace EcsRx.Views.Systems
{
    public abstract class PooledViewResolverSystem : ISetupSystem, IDisposable
    {
        private readonly IDictionary<Guid, object> _viewCache = new Dictionary<Guid, object>();
        private IDisposable _destructionSubscription;

        public IEventSystem EventSystem { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));
        public abstract IViewPool ViewPool { get; }

        protected PooledViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;

            _destructionSubscription = EventSystem.Receive<ComponentsRemovedEvent>()
                .Subscribe(x =>
                {
                    if (_viewCache.ContainsKey(x.Entity.Id) && x.Components.Any(y => y is ViewComponent))
                    { RecycleView(_viewCache[x.Entity.Id]); }
                });
        }

        protected abstract void OnViewRecycled(object view);
        protected abstract void OnViewAllocated(object view, IEntity entity);

        protected virtual void RecycleView(object viewToRecycle)
        {
            ViewPool.ReleaseInstance(viewToRecycle);
            OnViewRecycled(viewToRecycle);
        }

        protected virtual object AllocateView(IEntity entity)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            _viewCache.Add(entity.Id, viewToAllocate);
            OnViewAllocated(viewToAllocate, entity);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        { AllocateView(entity); }

        public virtual void Dispose()
        { _destructionSubscription.Dispose(); }
    }
}