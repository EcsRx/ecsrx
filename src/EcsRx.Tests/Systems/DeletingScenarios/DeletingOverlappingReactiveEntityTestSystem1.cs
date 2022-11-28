using System;
using System.Reactive.Linq;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingReactiveEntityTestSystem1 : IReactToEntitySystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentOne>();
        
        public IEntityCollection EntityCollection { get; }

        public DeletingOverlappingReactiveEntityTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public IObservable<IEntity> ReactToEntity(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber.Select(x => entity);  }

        public void Process(IEntity entity)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}