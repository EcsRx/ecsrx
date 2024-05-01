using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using SystemsRx.Scheduling;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingBasicEntitySystem2 : IBasicEntitySystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public void Process(IEntity entity, ElapsedTime elapsedTime)
        { throw new Exception("Should Not Be Called"); }
    }
}