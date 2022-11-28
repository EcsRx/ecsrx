using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingSetupTestSystem1 : ISetupSystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public IEntityCollection EntityCollection { get; }

        public DeletingSetupTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Setup(IEntity entity)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}