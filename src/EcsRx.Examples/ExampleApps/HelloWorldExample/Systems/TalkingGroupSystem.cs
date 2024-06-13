using System;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.HelloWorldExample.Components;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using R3;

namespace EcsRx.Examples.ExampleApps.HelloWorldExample.Systems
{
    public class TalkingGroupSystem : IReactToGroupSystem
    {
        public IGroup Group => new Group(typeof(CanTalkComponent));

        public Observable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(2)).Select(x => observableGroup); }

        public void Process(IEntity entity)
        {
            var canTalkComponent = entity.GetComponent<CanTalkComponent>();
            Console.WriteLine($"Entity says '{canTalkComponent.Message}' @ {DateTime.Now}");
        }
    }
}