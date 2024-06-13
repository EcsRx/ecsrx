using System;
using System.Threading;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using R3;

namespace EcsRx.Examples.ExampleApps.Performance.Systems
{
    public class ExampleReactToGroupSystem : IReactToGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(SimpleReadComponent), typeof(SimpleWriteComponent));
        
        public Observable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => observableGroup); }

        public void Process(IEntity entity)
        {
            var readComponent = entity.GetComponent<SimpleReadComponent>();
            var writeComponent = entity.GetComponent<SimpleWriteComponent>();
            writeComponent.WrittenValue = readComponent.StartingValue;
            Thread.Sleep(1); // Just to pretend there is something complex happening
        }
    }
}