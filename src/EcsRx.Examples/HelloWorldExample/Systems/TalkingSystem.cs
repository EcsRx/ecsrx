using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Examples.HelloWorldExample.Components;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;

namespace EcsRx.Examples.HelloWorldExample.Systems
{
    public class TalkingSystem : IReactToGroupSystem
    {
        public IGroup TargetGroup => new Group(typeof(CanTalkComponent));

        public IObservable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(2)).Select(x => observableGroup); }

        public void Execute(IEntity entity)
        {
            var canTalkComponent = entity.GetComponent<CanTalkComponent>();
            Console.WriteLine($"Entity says '{canTalkComponent.Message}' @ {DateTime.Now}");
        }
    }
}