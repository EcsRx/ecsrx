using System;
using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Views.Components;
using EcsRx.Views.ViewHandlers;

namespace EcsRx.Views.Systems
{
    public abstract class ViewResolverSystem : ISetupSystem, ITeardownSystem
    {
        public IEventSystem EventSystem { get; }

        public abstract IViewHandler ViewHandler { get; }

        public virtual IGroup Group => new Group(typeof(ViewComponent));

        protected ViewResolverSystem(IEventSystem eventSystem)
        {
            EventSystem = eventSystem;
        }

        protected virtual void OnViewRemoved(IEntity entity, ViewComponent viewComponent)
        { ViewHandler.DestroyView(viewComponent.View); }

        protected abstract void OnViewCreated(IEntity entity, ViewComponent viewComponent);

        public virtual void Setup(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            if (viewComponent.View != null) { return; }

            viewComponent.View = ViewHandler.CreateView();
            OnViewCreated(entity, viewComponent);
        }

        public virtual void Teardown(IEntity entity)
        {
            var viewComponent = entity.GetComponent<ViewComponent>();
            OnViewRemoved(entity, viewComponent);

        }
    }
}