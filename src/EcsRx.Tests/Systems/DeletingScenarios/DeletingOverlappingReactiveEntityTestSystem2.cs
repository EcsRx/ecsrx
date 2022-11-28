using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Tests.Models;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingOverlappingReactiveEntityTestSystem2 : IReactToEntitySystem
    {
        public IGroup Group => new Group()
            .WithComponent<ComponentWithReactiveProperty>()
            .WithComponent<TestComponentOne>();
        
        public IObservable<IEntity> ReactToEntity(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber.Select(x => entity); }

        public void Process(IEntity entity)
        { throw new Exception("Should Not Get Called"); }
    }
}