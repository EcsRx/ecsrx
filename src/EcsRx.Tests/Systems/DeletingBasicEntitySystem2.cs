using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems
{
    public class DeletingBasicEntitySystem2 : IBasicEntitySystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public void Process(IEntity entity)
        { throw new Exception("Should Not Be Called"); }
    }
}