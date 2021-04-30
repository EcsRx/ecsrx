using System.Diagnostics;
using SystemsRx.Threading;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Groups;
using EcsRx.Plugins.Batching.Batches;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.Batching.Factories;

namespace EcsRx.Plugins.Batching.Systems
{
    public abstract unsafe class BatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));
        
        private readonly IBatchBuilder<T1, T2> _batchBuilder;
        protected PinnedBatch<T1, T2> _batch;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2>();
        }

        protected override void RebuildBatch()
        {
            _batch.Dispose();
            _batch = _batchBuilder.Build(ObservableGroup);
        }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batch.Batches.Length, i =>
                {
                    ref var batch = ref _batch.Batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2);
                });
                return;
            }

            for (var i = 0; i < _batch.Batches.Length; i++)
            {
                ref var batch = ref _batch.Batches[i];
                Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2);
            }
        }
    }
    
    public abstract unsafe class BatchedSystem<T1, T2, T3> : ManualBatchedSystem
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3));
        
        private readonly IBatchBuilder<T1, T2, T3> _batchBuilder;
        protected PinnedBatch<T1, T2, T3> _batch;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3>();
        }

        protected override void RebuildBatch()
        {
            _batch.Dispose();
            _batch = _batchBuilder.Build(ObservableGroup);
        }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batch.Batches.Length, i =>
                {
                    ref var batch = ref _batch.Batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, ref *batch.Component3);
                });
                return;
            }

            for (var i = 0; i < _batch.Batches.Length; i++)
            {
                ref var batch = ref _batch.Batches[i];
                Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, ref *batch.Component3);
            }
        }
    }
    
    public abstract unsafe class BatchedSystem<T1, T2, T3, T4> : ManualBatchedSystem
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        
        private readonly IBatchBuilder<T1, T2, T3, T4> _batchBuilder;
        protected PinnedBatch<T1, T2, T3, T4> _batch;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4>();
        }

        protected override void RebuildBatch()
        {
            _batch.Dispose();
            _batch = _batchBuilder.Build(ObservableGroup);
        }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batch.Batches.Length, i =>
                {
                    ref var batch = ref _batch.Batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                        ref *batch.Component3, ref *batch.Component4);
                });
                return;
            }

            for (var i = 0; i < _batch.Batches.Length; i++)
            {
                ref var batch = ref _batch.Batches[i];
                Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                    ref *batch.Component3, ref *batch.Component4);
            }
        }
    }
    
    public abstract unsafe class BatchedSystem<T1, T2, T3, T4, T5> : ManualBatchedSystem
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        
        private readonly IBatchBuilder<T1, T2, T3, T4, T5> _batchBuilder;
        protected PinnedBatch<T1, T2, T3, T4, T5> _batch;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4, T5>();
        }

        protected override void RebuildBatch()
        {
            _batch.Dispose();
            _batch = _batchBuilder.Build(ObservableGroup);
        }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batch.Batches.Length, i =>
                {
                    ref var batch = ref _batch.Batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                        ref *batch.Component3, ref *batch.Component4, ref *batch.Component5);
                });
                return;
            }

            for (var i = 0; i < _batch.Batches.Length; i++)
            {
                ref var batch = ref _batch.Batches[i];
                Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                    ref *batch.Component3, ref *batch.Component4, ref *batch.Component5);
            }
        }
    }
    
    public abstract unsafe class BatchedSystem<T1, T2, T3, T4, T5, T6> : ManualBatchedSystem
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        
        private readonly IBatchBuilder<T1, T2, T3, T4, T5, T6> _batchBuilder;
        protected PinnedBatch<T1, T2, T3, T4, T5, T6> _batch;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5, ref T6 component6);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler, IObservableGroupManager observableGroupManager) : base(componentDatabase,
            componentTypeLookup, threadHandler, observableGroupManager)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
        }

        protected override void RebuildBatch()
        {
            _batch.Dispose();
            _batch = _batchBuilder.Build(ObservableGroup);
        }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batch.Batches.Length, i =>
                {
                    ref var batch = ref _batch.Batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                        ref *batch.Component3, ref *batch.Component4, ref *batch.Component5,
                        ref *batch.Component6);
                });
                return;
            }

            for (var i = 0; i < _batch.Batches.Length; i++)
            {
                ref var batch = ref _batch.Batches[i];
                Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                    ref *batch.Component3, ref *batch.Component4, ref *batch.Component5,
                    ref *batch.Component6);
            }
        }
    }
}