using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Pools;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.Pooling;
using EcsRx.Views.ViewHandlers;

namespace EcsRx.Views.Systems
{
    public abstract class ViewResolverSystem : ISetupSystem
    {
        public IViewHandler ViewHandler { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));

        protected ViewResolverSystem(IViewHandler viewHandler)
        {
            ViewHandler = viewHandler;
        }

        public abstract object ResolveView(IEntity entity);

        public void Setup(IEntity entity)
        { ViewHandler.SetupView(entity, ResolveView); }
    }
    
    public abstract class PooleddViewResolverSystem : ISetupSystem
    {
        public IViewHandler ViewHandler { get; }
        public IViewPool ViewPool { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));

        protected PooleddViewResolverSystem(IViewHandler viewHandler, IViewPool viewPool)
        {
            ViewHandler = viewHandler;
            ViewPool = viewPool;
        }

        protected abstract void OnViewRecycled(object view);
        protected abstract void OnViewAllocated(object view);

        protected virtual void RecycleView(object viewToRecycle)
        {
            ViewPool.ReleaseInstance(viewToRecycle);
            OnViewRecycled(viewToRecycle);
        }

        protected virtual object AllocateView(IEntity entity, IPool pool)
        {
            var viewToAllocate = ViewPool.AllocateInstance();
            OnViewAllocated(viewToAllocate);
            return viewToAllocate;
        }

        public void Setup(IEntity entity)
        { ViewHandler.SetupView(entity,  ); }
    }
}