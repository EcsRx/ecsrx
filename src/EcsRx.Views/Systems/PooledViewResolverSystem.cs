using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.Pooling;
using EcsRx.Views.ViewHandlers;

namespace EcsRx.Views.Systems
{
    public abstract class PooledViewResolverSystem : ISetupSystem
    {
        public IEntityViewHandler EntityViewHandler { get; }
        public IViewPool ViewPool { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));

        protected PooledViewResolverSystem(IEntityViewHandler entityViewHandler, IViewPool viewPool)
        {
            EntityViewHandler = entityViewHandler;
            ViewPool = viewPool;
        }

        protected abstract void OnViewRecycled(object view);
        protected abstract void OnViewAllocated(object view);

        protected virtual void RecycleView(object viewToRecycle)
        {
            ViewPool.ReleaseInstance(viewToRecycle);
            OnViewRecycled(viewToRecycle);
        }

        protected virtual object AllocateView(IEntity entity)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            OnViewAllocated(viewToAllocate);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        { AllocateView(entity); }
    }
}