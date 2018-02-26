using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.Pooling;

namespace EcsRx.Views.Systems
{
    public abstract class PooledViewResolverSystem : ISetupSystem
    {
        public abstract IViewPool ViewPool { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));
        
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
            OnViewAllocated(viewToAllocate, entity);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        { AllocateView(entity); }
    }
}