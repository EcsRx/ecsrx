using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.ViewHandlers;

namespace EcsRx.Views.Systems
{
    public abstract class ViewResolverSystem : ISetupSystem
    {
        public IEntityViewHandler EntityViewHandler { get; }

        public virtual IGroup TargetGroup => new Group(typeof(ViewComponent));

        protected ViewResolverSystem(IEntityViewHandler entityViewHandler)
        { EntityViewHandler = entityViewHandler; }

        public void Setup(IEntity entity)
        { EntityViewHandler.SetupView(entity); }
    }
}