using System.Diagnostics;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Groups;
using EcsRx.Plugins.Batching.Batches;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.Batching.Factories;
using EcsRx.Threading;

namespace EcsRx.Plugins.Batching.Systems
{
    public abstract unsafe class BatchedSystem<T1, T2> : ManualBatchedSystem
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public override IGroup Group { get; } = new Group(typeof(T1), typeof(T2));
        
        private readonly IBatchBuilder<T1, T2> _batchBuilder;
        protected Batch<T1, T2>[] _batches;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase,
            componentTypeLookup, threadHandler)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    ref var batch = ref _batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                ref var batch = ref _batches[i];
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
        protected Batch<T1, T2, T3>[] _batches;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase,
            componentTypeLookup, threadHandler)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    ref var batch = ref _batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, ref *batch.Component3);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                ref var batch = ref _batches[i];
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
        protected Batch<T1, T2, T3, T4>[] _batches;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase,
            componentTypeLookup, threadHandler)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    ref var batch = ref _batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                        ref *batch.Component3, ref *batch.Component4);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                ref var batch = ref _batches[i];
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
        protected Batch<T1, T2, T3, T4, T5>[] _batches;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase,
            componentTypeLookup, threadHandler)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4, T5>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    ref var batch = ref _batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                        ref *batch.Component3, ref *batch.Component4, ref *batch.Component5);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                ref var batch = ref _batches[i];
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
        protected Batch<T1, T2, T3, T4, T5, T6>[] _batches;
        
        protected abstract void Process(int entityId, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4, ref T5 component5, ref T6 component6);

        protected BatchedSystem(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup,
            IBatchBuilderFactory batchBuilderFactory, IThreadHandler threadHandler) : base(componentDatabase,
            componentTypeLookup, threadHandler)
        {
            _batchBuilder = batchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
        }

        protected override void RebuildBatch()
        { _batches = _batchBuilder.Build(ObservableGroup); }
        
        protected override void ProcessBatch()
        {
            if (ShouldParallelize)
            {
                ThreadHandler.For(0, _batches.Length, i =>
                {
                    ref var batch = ref _batches[i];
                    Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                        ref *batch.Component3, ref *batch.Component4, ref *batch.Component5,
                        ref *batch.Component6);
                });
                return;
            }

            for (var i = 0; i < _batches.Length; i++)
            {
                ref var batch = ref _batches[i];
                Process(batch.EntityId, ref *batch.Component1, ref *batch.Component2, 
                    ref *batch.Component3, ref *batch.Component4, ref *batch.Component5,
                    ref *batch.Component6);
            }
        }
    }
}