using SystemsRx.Scheduling;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Components;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Systems
{
    public class PlayerStateUpdaterEntitySystem : IBasicEntitySystem
    {
        public IGroup Group { get; } = new Group(typeof(PlayerStateComponent));
        
        public ITimeTracker TimeTracker { get; }

        public PlayerStateUpdaterEntitySystem(ITimeTracker timeTracker)
        {
            TimeTracker = timeTracker;
        }

        public void Process(IEntity entity)
        {
            var playerState = entity.GetComponent<PlayerStateComponent>();
            playerState.PlayTime += TimeTracker.ElapsedTime.DeltaTime;
        }
    }
}