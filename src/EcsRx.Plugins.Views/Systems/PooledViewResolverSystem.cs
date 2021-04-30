using SystemsRx.Events;
using SystemsRx.Systems;
using SystemsRx.Systems.Conventional;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.Views.Components;
using EcsRx.Plugins.Views.Pooling;
using EcsRx.Systems;

namespace EcsRx.Plugins.Views.Systems
{
    public abstract class PooledViewResolverSystem : IViewResolverSystem, IManualSystem, IGroupSystem
    {
        public IEventSystem EventSystem { get; }

        public virtual IGroup Group => new Group(typeof(ViewComponent));
        public IViewPool ViewPool { get; private set; }

        protected PooledViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        protected abstract IViewPool CreateViewPool();
        protected abstract void OnPoolStarting();
        protected abstract void OnViewRecycled(object view, IEntity entity);
        protected abstract void OnViewAllocated(object view, IEntity entity);

        protected virtual void RecycleView(IEntity entity, ViewComponent viewComponent)
        {
            var view = viewComponent.View;
            ViewPool.ReleaseInstance(view);
            viewComponent.View = null;
            OnViewRecycled(view, entity);
        }

        protected virtual object AllocateView(IEntity entity, ViewComponent viewComponent)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            viewComponent.View = viewToAllocate;
            OnViewAllocated(viewToAllocate, entity);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            AllocateView(entity, viewComponent);
        }

        public virtual void Teardown(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            RecycleView(entity, viewComponent);
        }

        public void StartSystem()
        {
            ViewPool = CreateViewPool();
            OnPoolStarting();
        }

        public void StopSystem()
        { ViewPool.EmptyPool(); }
    }
}