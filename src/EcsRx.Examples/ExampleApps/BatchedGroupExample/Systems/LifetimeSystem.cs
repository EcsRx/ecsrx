using System;
using System.Reactive.Linq;
using EcsRx.Blueprints;
using EcsRx.Collections;
using EcsRx.Collections.Database;
using EcsRx.Collections.Entity;
using EcsRx.Examples.ExampleApps.BatchedGroupExample.Blueprints;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;

namespace EcsRx.Examples.ExampleApps.BatchedGroupExample.Systems
{
    public class SpawnerSystem : IManualSystem
    {
        private IDisposable _sub;
        private IBlueprint _blueprint = new MoveableBlueprint();
        
        public IGroup Group { get; } = new EmptyGroup();       
        public IEntityCollection DefaultCollection { get; }

        public SpawnerSystem(IEntityDatabase entityDatabase)
        { DefaultCollection = entityDatabase.GetCollection(); }

        public void StartSystem(IObservableGroup observableGroup)
        { _sub = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => Spawn()); }

        public void Spawn()
        { DefaultCollection.CreateEntity(_blueprint); }

        public void StopSystem(IObservableGroup observableGroup)
        { _sub.Dispose(); }
    }
}