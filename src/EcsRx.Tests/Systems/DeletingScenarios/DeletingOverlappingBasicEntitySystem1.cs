using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using SystemsRx.Scheduling;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingBasicEntitySystem1 : IBasicEntitySystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentThree>();
        
        public IEntityCollection EntityCollection { get; }

        public DeletingOverlappingBasicEntitySystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Process(IEntity entity, ElapsedTime elapsedTime)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}