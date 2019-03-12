using System;
using System.Reactive.Linq;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Plugins.Batching.Factories;
using EcsRx.Plugins.Batching.Systems;
using EcsRx.Threading;

namespace EcsRx.Examples.ExampleApps.Performance.Systems
{
    public class ExampleBatchedGroupSystem : ReferenceBatchedSystem<SimpleReadComponent, SimpleWriteComponent>
    {
        public ExampleBatchedGroupSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler)
        {}

        protected override IObservable<bool> ReactWhen()
        {
            return Observable.Never<bool>();
        }

        protected override IObservable<IEntity> ProcessGroupSubscription(IObservable<IEntity> groupChange)
        { return groupChange.Throttle(TimeSpan.FromMilliseconds(10)); }

        protected override void Process(int EntityId, SimpleReadComponent component1, SimpleWriteComponent component2)
        {}
    }
}