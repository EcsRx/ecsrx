using System;
using System.Reactive.Linq;
using SystemsRx.Threading;
using EcsRx.Collections;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Plugins.Batching.Factories;
using EcsRx.Plugins.Batching.Systems;

namespace EcsRx.Examples.ExampleApps.Performance.Systems
{
    public class ExampleBatchedSystem : ReferenceBatchedSystem<SimpleReadComponent, SimpleWriteComponent>
    {
        public ExampleBatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, 
            IReferenceBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager)
            : base(componentDatabase, componentTypeLookup, batchBuilderFactory, threadHandler, observableGroupManager)
        {}

        protected override IObservable<bool> ReactWhen()
        { return Observable.Never<bool>(); }

        // This shows that every time the group changes, it should throttle (actually debounce) and run after 10ms
        protected override IObservable<IEntity> ProcessGroupSubscription(IObservable<IEntity> groupChange)
        { return groupChange.Throttle(TimeSpan.FromMilliseconds(10)); }

        protected override void Process(int EntityId, SimpleReadComponent component1, SimpleWriteComponent component2)
        {}
    }
}