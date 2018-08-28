using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.Pooling;

namespace EcsRx.Views.Systems
{
    public abstract class PooledViewResolverSystem : IViewResolverSystem
    {
        public IEventSystem EventSystem { get; }

        public virtual IGroup Group => new Group(typeof(ViewComponent));
        public IViewPool ViewPool { get; private set; }

        protected PooledViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
            SetupViewPool();
        }

        protected void SetupViewPool()
        {
            ViewPool = CreateViewPool();
            OnPoolStarting();
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
    }
}