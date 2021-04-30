using System;
using System.Reactive.Linq;
using SystemsRx.Systems.Conventional;
using EcsRx.Blueprints;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Blueprints;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class SpawnerSystem : IManualSystem
    {
        private IDisposable _sub;
        private IBlueprint _blueprint = new MoveableBlueprint();
            
        public IEntityCollection DefaultCollection { get; }

        public SpawnerSystem(IEntityDatabase entityDatabase)
        { DefaultCollection = entityDatabase.GetCollection(); }

        public void StartSystem()
        { _sub = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => Spawn()); }

        public void Spawn()
        { DefaultCollection.CreateEntity(_blueprint); }

        public void StopSystem()
        { _sub.Dispose(); }
    }
}