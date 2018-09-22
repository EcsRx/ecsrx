using System;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Tests.Models;
using EcsRx.Views.Components;
using EcsRx.Views.Systems;
using EcsRx.Views.ViewHandlers;

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