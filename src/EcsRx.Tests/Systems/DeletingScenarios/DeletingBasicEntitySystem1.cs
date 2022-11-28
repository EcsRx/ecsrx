using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingBasicEntitySystem1 : IBasicEntitySystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();
        
        public IEntityCollection EntityCollection { get; }

        public DeletingBasicEntitySystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public void Process(IEntity entity)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}