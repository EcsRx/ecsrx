using System;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingReactiveDataTestSystem1 : IReactToDataSystem<int>
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();
        public IEntityCollection EntityCollection { get; }

        public DeletingReactiveDataTestSystem1(IEntityCollection entityCollection)
        { EntityCollection = entityCollection; }

        public IObservable<int> ReactToData(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber; }

        public void Process(IEntity entity, int reactionData)
        { EntityCollection.RemoveEntity(entity.Id); }
    }
}