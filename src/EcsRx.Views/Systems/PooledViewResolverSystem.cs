using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using EcsRx.Views.Components;

namespace EcsRx.Views.Systems
{
    public abstract class PooledViewResolverSystem : ISetupSystem, IDisposable
    {
        public IPoolManager PoolManager { get; }
        public IEventSystem EventSystem { get; }

        private readonly IDictionary<Guid, object> _viewCache;
        private readonly IDisposable _entitySubscription;

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));

        protected PooledViewResolverSystem(IPoolManager poolManager, IEventSystem eventSystem)
        {
            PoolManager = poolManager;
            EventSystem = eventSystem;

            _viewCache = new Dictionary<Guid, object>();

            _entitySubscription = EventSystem
                .Receive<ComponentsRemovedEvent>()
                .Where(x => x.Component is ViewComponent && _viewCache.ContainsKey(x.Entity.Id))
                .Subscribe(x =>
                {
                    var view = _viewCache[x.Entity.Id];
                    RecycleView(view);
                    _viewCache.Remove(x.Entity.Id);
                });

            PrefabTemplate = ResolvePrefabTemplate();
        }

        protected abstract object ResolvePrefabTemplate();
        protected abstract void RecycleView(object viewToRecycle);
        protected abstract object AllocateView(IEntity entity, IPool pool);

        public virtual void Setup(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            if (viewComponent.View != null) { return; }

            var containingPool = PoolManager.GetContainingPoolFor(entity);
            var viewObject = AllocateView(entity, containingPool);
            viewComponent.View = viewObject;

            _viewCache.Add(entity.Id, viewObject);
        }

        public void Dispose()
        { _entitySubscription.Dispose(); }
    }
}