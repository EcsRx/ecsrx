using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingSetupTestSystem1 : ISetupSystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentTwo>();

        public IEntityCollection EntityCollection { get; }

        public DeletingOverlappingSetupTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Setup(IEntity entity)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}