using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.Pooling;

namespace EcsRx.Views.Systems
{
    public abstract class PooledViewResolverSystem : ISetupSystem, ITeardownSystem
    {
        public IEventSystem EventSystem { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));
        public abstract IViewPool ViewPool { get; }

        protected PooledViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        protected abstract void OnViewRecycled(object view, IEntity entity);
        protected abstract void OnViewAllocated(object view, IEntity entity);

        protected virtual void RecycleView(IEntity entity, ViewComponent viewComponent)
        {
            ViewPool.ReleaseInstance(viewComponent.View);
            OnViewRecycled(viewComponent.View, entity);
        }

        protected virtual object AllocateView(IEntity entity)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            OnViewAllocated(viewToAllocate, entity);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        { AllocateView(entity); }

        public virtual void Teardown(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            RecycleView(entity, viewComponent);
        }
    }
}