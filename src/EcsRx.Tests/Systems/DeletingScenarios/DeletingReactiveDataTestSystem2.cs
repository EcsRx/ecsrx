using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingReactiveDataTestSystem2 : IReactToDataSystem<int>
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();

        public IObservable<int> ReactToData(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber; }

        public void Process(IEntity entity, int reactionData)
        { throw new Exception("Should Not Get Called"); }
    }
}