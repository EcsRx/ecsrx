using System;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Lookups;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using R3;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class LoggingSystem : IReactToGroupExSystem
    {
        public IGroup Group { get; } = new Group(typeof(NameComponent), typeof(PositionComponent));

        public Observable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => observableGroup); }
        
        public void Process(IEntity entity)
        {
            var nameComponent = entity.GetComponent<NameComponent>();
            var positionComponent = entity.GetComponent<PositionComponent>(ComponentLookupTypes.PositionComponentId);
            Console.WriteLine($"{nameComponent.Name} - {positionComponent.Position}");
        }

        public void BeforeProcessing()
        {
            Console.SetCursorPosition(0,0);
            Console.Clear();
        }

        public void AfterProcessing() {}
    }
}