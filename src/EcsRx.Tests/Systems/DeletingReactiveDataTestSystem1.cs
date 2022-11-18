using System;
using System.Reactive.Linq;
using EcsRx.Collections;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Tests.Models;
using SystemsRx.ReactiveData;

namespace EcsRx.Tests.Systems
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