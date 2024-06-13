using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;
using EcsRx.Tests.Models;
using R3;

namespace EcsRx.Tests.Systems.DeletingScenarios
{
    public class DeletingReactiveEntityTestSystem2 : IReactToEntitySystem
    {
        public IGroup Group => new Group().WithComponent<ComponentWithReactiveProperty>();
        
        public Observable<IEntity> ReactToEntity(IEntity entity)
        { return entity.GetComponent<ComponentWithReactiveProperty>().SomeNumber.Select(x => entity); }

        public void Process(IEntity entity)
        { throw new Exception("Should Not Get Called"); }
    }
}