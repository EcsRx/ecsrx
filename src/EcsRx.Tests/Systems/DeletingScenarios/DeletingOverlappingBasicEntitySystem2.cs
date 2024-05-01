using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using SystemsRx.Scheduling;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingBasicEntitySystem2 : IBasicEntitySystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentThree>();

        public void Process(IEntity entity, ElapsedTime elapsedTime)
        { throw new Exception("Should Not Be Called"); }
    }
}