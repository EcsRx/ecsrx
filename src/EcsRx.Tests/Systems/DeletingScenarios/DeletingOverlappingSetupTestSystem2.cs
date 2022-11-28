using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingSetupTestSystem2 : ISetupSystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentTwo>();

        public void Setup(IEntity entity)
        { throw new Exception("Should Not Get Called"); }
    }
}