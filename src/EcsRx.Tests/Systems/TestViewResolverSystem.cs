using System;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Groups;
using EcsRx.Plugins.Views.Components;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Plugins.Views.ViewHandlers;

namespace EcsRx.Tests.Systems
{
    public class TestViewResolverSystem : ViewResolverSystem
    {
        public override IViewHandler ViewHandler { get; }
        public override IGroup Group { get; }

        public Action<IEntity> OnSetup { get; set; }
        public Action<IEntity> OnTeardown { get; set; }
        
        public TestViewResolverSystem(IEventSystem eventSystem, IGroup group) : base(eventSystem)
        {
            Group = group;
        }
        
        protected override void OnViewCreated(IEntity entity, ViewComponent viewComponent)
        {
            
        }

        public override void Setup(IEntity entity)
        {
            OnSetup?.Invoke(entity);
        }
        
        public override void Teardown(IEntity entity)
        {
            OnTeardown?.Invoke(entity);
        }
    }
}