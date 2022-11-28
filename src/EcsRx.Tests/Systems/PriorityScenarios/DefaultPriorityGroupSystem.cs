﻿using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Systems;

namespace EcsRx.Tests.Systems.PriorityScenarios
{
    public class DefaultPriorityGroupSystem : IGroupSystem
    {
        public IGroup Group => null;
    }
    
    public class DefaultPrioritySetupSystem : ISetupSystem
    {
        public IGroup Group => null;
        public void Setup(IEntity entity){}
    }
    
    public class DefaultPriorityViewResolverSystem : IViewResolverSystem
    {
        public IGroup Group => null;
        public void Teardown(IEntity entity){}
        public void Setup(IEntity entity){}
    }
}