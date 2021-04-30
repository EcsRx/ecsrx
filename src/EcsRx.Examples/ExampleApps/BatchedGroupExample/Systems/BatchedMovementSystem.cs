using System;
using System.Numerics;
using System.Reactive.Linq;
using SystemsRx.Threading;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Components;
using EcsRx.Plugins.Batching.Factories;
using EcsRx.Plugins.Batching.Systems;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class BatchedMovementSystem : BatchedSystem<PositionComponent, MovementSpeedComponent>
    {
        public BatchedMovementSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler)
        {}

        protected override IObservable<bool> ReactWhen()
        { return Observable.Interval(TimeSpan.FromSeconds(0.5f)).Select(x => true); }

        protected override void Process(int entityId, ref PositionComponent positionComponent, ref MovementSpeedComponent movementSpeedComponent)
        {
            positionComponent.Position += Vector3.One * movementSpeedComponent.Speed;
        }
    }
}